using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorker;   //Thread automatskog upravljanja
        private AutoResetEvent automationTrigger;	//Properti koji vrsi sihronizaciju izmedju nasih niti
        private IStorage storage;  //Provjeravamo stanje nasih prekidaca --> 0 ili 1
		private IProcessingManager processingManager;  //Preko njega zadajemo write komandu
		private int delayBetweenCommands;  //DBC automatizacije
        private IConfiguration configuration;  //Pristupamo svakom signalu

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
			this.delayBetweenCommands = configuration.DelayBetweenCommands;		//dobija vrijednost iz konfiguracije
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorker = new Thread(AutomationWorker_DoWork);
			automationWorker.Name = "Aumation Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorker.Start();
		}


		private void AutomationWorker_DoWork()	   //OVDJE SE VRSI AUTOMATIZACIJA
		{
			EGUConverter eguConverter = new EGUConverter();		   //za konvertovanje iz sirovih
			//tip signala i njegovu adresu
			PointIdentifier analogOutput1 = new PointIdentifier(PointType.ANALOG_OUTPUT, 1000);	  //pozicija kapije
			PointIdentifier digitalInput1 = new PointIdentifier(PointType.DIGITAL_INPUT, 2000);   //indikator prepreke
			PointIdentifier digitalOutput1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3000); //open taster
			PointIdentifier digitalOutput2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 3001); //close taster

			List<PointIdentifier> pointList = new List<PointIdentifier>()  //ubacujemo u listu
			{
				analogOutput1, digitalInput1, digitalOutput1, digitalOutput2
			};

			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(pointList);	//dobili trenutne vrijednosti za nase signale

				if (points[2].RawValue == 1)  //ukljucen OPEN taster
				{
					int value = (int)eguConverter.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue);	 //vrijednost u inzinjerskim

					if (value > points[0].ConfigItem.LowLimit)	 //ako kapija nije ispod LowLimit
					{
						value -= 10; //otvaraj kapiju
						processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, value);	  //proslijedi
					}
					else	//ako je kapija ispod ili dosla do LowLimit
					{
						processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[2].ConfigItem.StartAddress, 0);	//ugasi taster Open

						if (points[1].RawValue == 1) //ako je ukljucen indikator prepreke
						{
							processingManager.ExecuteWriteCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[1].ConfigItem.StartAddress, 0);	 //iskljuci indikator prepreke
							processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 1);	 //pocni zatvarat kapiju
						}
					}
				}

				if (points[3].RawValue == 1)  //ukljucen CLOSE taster
				{
					if (points[1].RawValue == 1)   //ako je ukljucen indikator prepreke
					{
						processingManager.ExecuteWriteCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[2].ConfigItem.StartAddress, 1);	  //ukljuci OPEN taster
						processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 0);	  //iskljuci CLOSE taster
					}
					else
					{
						int value = (int)eguConverter.ConvertToEGU(points[0].ConfigItem.ScaleFactor, points[0].ConfigItem.Deviation, points[0].RawValue); //vrijednost

						if (value < points[0].ConfigItem.HighLimit)	 //ako je ispod HighLimit
						{
							value += 10;   //zatvaraj
							processingManager.ExecuteWriteCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, value);	  //proslijedi
						}
						else  //ako je kapija presla HighLimit
						{
							processingManager.ExecuteWriteCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 0);	//ugasi CLOSE taster
						}
					}
				}

				automationTrigger.WaitOne(delayBetweenCommands); //koliko vremena prodje izmedju dva komandovanja
			}

		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
