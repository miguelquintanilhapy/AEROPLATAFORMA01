using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace magal.Models
{
    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Mudamos para public para que a ViewModel possa disparar atualizações manuais
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}