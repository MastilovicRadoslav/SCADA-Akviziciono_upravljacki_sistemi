using System.ComponentModel;

namespace dCom.ViewModel
{
	public class ViewModelBase : INotifyPropertyChanged
	{
		internal void OnPropertyChanged(string prop)
		{
			if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}