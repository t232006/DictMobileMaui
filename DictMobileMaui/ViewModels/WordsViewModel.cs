using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DictMobile.models;
using IndDictionary;

namespace DictMobile.ViewModels
{
    public class WordsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<dict> wordsList = new ObservableCollection<dict>();
        private bool _showAll;
        private WhatToShow _wts;
        private bool _side;

        public ObservableCollection<dict> WordsList
        {
            get => wordsList;
            set
            {
                wordsList = value;
                OnPropertyChanged(nameof(WordsList));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public WordsViewModel(bool showAll, WhatToShow wts, bool side)
        {
            _showAll = showAll; _wts = wts; _side = side;
        }

        // Загружает список слов из базы (на фоновой задаче) и обновляет коллекцию на UI-потоке
        public async Task Search(string needle)
        {
            if (string.IsNullOrEmpty(needle))
            {
                var result = await App.Database.showTableDictAsync(_showAll, _wts);
                WordsList = new ObservableCollection<dict>(result);
            }
            WordsList = _side
                ? await App.Database.findRecordsAsync(needle, f => f.Translation)
                : await App.Database.findRecordsAsync(needle, f => f.Word);
        }
        public async Task LoadAsync()
        {
            var items = await Task.Run(() =>
            {
                var raw = App.Database.showTableDict(_showAll, _wts);
                return _side ? raw.OrderBy(t => t.Translation).ToList() : raw.OrderBy(t => t.Word).ToList();
            });

            WordsList = new ObservableCollection<dict>(items);
        }

        // Добавляет или обновляет запись в существующей коллекции (для мгновенного отображения новых записей)
        public void AddOrUpdate(dict item)
        {
            if (item == null) return;
            var existing = wordsList.FirstOrDefault(w => w.id == item.id);
            if (existing != null)
            {
                var idx = wordsList.IndexOf(existing);
                wordsList[idx] = item;
                OnPropertyChanged(nameof(WordsList));
            }
            else
            {
                wordsList.Add(item);
                OnPropertyChanged(nameof(WordsList));
            }
        }

        public void Remove(int id)
        {
            var existing = wordsList.FirstOrDefault(w => w.id == id);
            if (existing != null)
            {
                wordsList.Remove(existing);
                OnPropertyChanged(nameof(WordsList));
            }
        }
    }
}
