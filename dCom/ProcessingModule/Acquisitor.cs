﻿using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for periodic polling.
    /// </summary>
    public class Acquisitor : IDisposable
	{
		private AutoResetEvent acquisitionTrigger;//Sihronizacija svih niti koji se koriste u ovom projektu, pomocu njega cemo kasnije simulirati vrijeme jedne sekunde
        private IProcessingManager processingManager;//Izvrsava operacije Read, Write
        private Thread acquisitionWorker;//Thread u kome se vrti akvizicija
		private IStateUpdater stateUpdater;//Stanje Thread
		private IConfiguration configuration;//Polje koje definise konfiguraciju sistema

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
        /// <param name="configuration">The configuration.</param>
		public Acquisitor(AutoResetEvent acquisitionTrigger, IProcessingManager processingManager, IStateUpdater stateUpdater, IConfiguration configuration)
		{
			this.stateUpdater = stateUpdater;
			this.acquisitionTrigger = acquisitionTrigger;
			this.processingManager = processingManager;
			this.configuration = configuration;
			this.InitializeAcquisitionThread();
			this.StartAcquisitionThread();
		}

		#region Private Methods

        /// <summary>
        /// Initializes the acquisition thread.
        /// </summary>
		private void InitializeAcquisitionThread()
		{
			this.acquisitionWorker = new Thread(Acquisition_DoWork);
			this.acquisitionWorker.Name = "Acquisition thread";
		}

        /// <summary>
        /// Starts the acquisition thread.
        /// </summary>
		private void StartAcquisitionThread()
		{
			acquisitionWorker.Start();
		}

        /// <summary>
        /// Acquisitor thread logic.
        /// </summary>
		private void Acquisition_DoWork()//pustena u Thread, i ona se izvrsava u Thread, tu je potrebno napraviti nasu akviziciju
		{
			//TO DO: IMPLEMENT
			//u neku listu iscitati konfiguraciju
			//dobijamo cijelu konfiguraciju, kao jednu listu klase configItem(definise jedan red u konfiguraciji)
			//while ty, petlja da dobijemo beskonacnu akviziciju dok radi nasa aplikacija
			//simulacija jedne sekunde pomocu klase acquistionTrigger
			//foreach prolazak kroz cijelu nasu listu koju smo ocitali u konfiguraciju
			//za taj cijeli configItem uvecacemo prvo polje koje kaze kaze koliko je vremena proslo od prehodne akvizicije
			//poredimo sa poljem i kraj ako je doslo do kraja
			List<IConfigItem> help = new List<IConfigItem>();//napravimo listu

			help = this.configuration.GetConfigurationItems();	//dobijamo listu configItem koji se koristi

			while (true)
			{
				acquisitionTrigger.WaitOne();	//sacekamo jednu sekundu

				foreach(IConfigItem item in help) {	  //prolazimo kroz svaki item
					item.SecondsPassedSinceLastPoll++; //povecavamo njegovu vrijednost, sekundi koje su prosle
					if(item.SecondsPassedSinceLastPoll == item.AcquisitionInterval)	//ako je izvrsen intreval, ispunjen
					{
						processingManager.ExecuteReadCommand(item,	//izvrsimo Read komandu sa parametrima koji su potrebni(item, Id, 
							configuration.GetTransactionId(),
							configuration.UnitAddress,
							item.StartAddress,
							item.NumberOfRegisters);
						item.SecondsPassedSinceLastPoll = 0;
					}
				}
			}
        }

        #endregion Private Methods

        /// <inheritdoc />
        public void Dispose()
		{
			acquisitionWorker.Abort();
        }
	}
}