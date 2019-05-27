using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ComicViewer.ViewModels;
using ComicViewer.CtModels;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using ComicViewer.Views;
using System.Diagnostics;

namespace ComicViewer.ViewModels
{
    public class ComicDetailViewModel : BaseViewModel
    {
        private CtComic _oldComic;
        private ImageSource _imageSource;
        const int _downloadImageTimeoutInSeconds = 15;
        readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

        private CtComic _tempComic;
        public ImageSource CoverImage
        {
            get
            {
                if (_tempComic.CoverUrl != null)
                {
                    if (_imageSource == null)
                        _imageSource = ImageSource.FromUri(new Uri(_tempComic.CoverUrl));
                    return _imageSource;
                }
                else
                    return null;
            }
        }
        private bool _lang;
        public bool Lang
        {
            get
            {
                return _lang;
            }
            set
            {
                _lang = value;
                OnAllPropertyChanged();
            }
        }
        public string WorkId
        {
            get
            {
                return _tempComic.Workid;
            }
        }
        public string ComicTitle
        {
            get
            {
                return Lang ? _tempComic.TitleKr : _tempComic.TitleEn;
            }
            set
            {
                if (Lang)
                    _tempComic.TitleKr = value;
                else
                    _tempComic.TitleEn = value;
                OnPropertyChanged();
            }
        }
        public string ComicArtist
        {
            get
            {
                return Lang ? _tempComic.ArtistKr : _tempComic.ArtistEn;
            }
        }
        public string ComicGroup
        {
            get
            {
                return Lang ? _tempComic.GroupKr : _tempComic.GroupEn;
            }
        }
        public string ComicSeries
        {
            get
            {
                return Lang ? _tempComic.SeriesKr : _tempComic.SeriesEn;
            }
        }
        public string ComicCharacter
        {
            get
            {
                return Lang ? _tempComic.CharacterKr : _tempComic.CharacterEn;
            }
        }
        public string ComicTag
        {
            get
            {
                return Lang ? _tempComic.TagKr : _tempComic.TagEn;
            }
        }
        public int ComicPage
        {
            get
            {
                return _tempComic.Page;
            }
        }
        public string ComicLanguage
        {
            get
            {
                return _tempComic.Language;
            }
        }
        public string ComicType1
        {
            get
            {
                return Lang ? _tempComic.TypeKr : _tempComic.TypeEn;
            }
        }
        public string ComicType2
        {
            get
            {
                return Lang ? _tempComic.Type2Kr : _tempComic.Type2En;
            }
        }
        public string UploadedDate
        {
            get
            {
                return _tempComic.UploadedDateStr;
            }
        }
        public string ModifiedDate
        {
            get
            {
                return _tempComic.ModifiedDateStr;
            }
        }
        public bool IsTranslated
        {
            get
            {
                return _tempComic.IsTranslated;
            }
            set
            {
                _tempComic.IsTranslated = value;
                OnPropertyChanged();
            }
        }
        public bool IsHidden
        {
            get
            {
                return _tempComic.IsHidden;
            }
            set
            {
                _tempComic.IsHidden = value;
                OnPropertyChanged();
            }
        }
        public bool IsFavorite
        {
            get
            {
                return _tempComic.IsFavorite;
            }
            set
            {
                _tempComic.IsFavorite = value;
                OnPropertyChanged();
            }
        }
        public string CoverUrl
        {
            get
            {
                return _tempComic.CoverUrl;
            }
            set
            {
                _tempComic.CoverUrl = value;
                OnPropertyChanged();
            }
        }
        public string ImageUrl
        {
            get
            {
                return _tempComic.ImageUrl;
            }
            set
            {
                _tempComic.ImageUrl = value;
                OnPropertyChanged();
            }
        }

        bool _loading;
        public bool Loading
        {
            get
            {
                return _loading;
            }

            set
            {
                _loading = value;
                OnPropertyChanged();
            }
        }

        private readonly IWebConnection _webif;
        public ComicDetailViewModel(CtComic comic, IWebConnection webif)
        {
            
            _webif = webif;
            _oldComic = comic;
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

            Lang = (RootApp.CtProgramLanguage == CtLanguage.KOREAN);
            Loading = true;
            _tempComic = _oldComic.Clone() as CtComic;
        }

        public void OnAllPropertyChanged()
        {
            OnPropertyChanged("ComicTitle");
            OnPropertyChanged("ComicArtist");
            OnPropertyChanged("ComicGroup");
            OnPropertyChanged("ComicSeries");
            OnPropertyChanged("ComicCharacter");
            OnPropertyChanged("ComicTag");
            OnPropertyChanged("ComicType1");
            OnPropertyChanged("ComicType2");
            OnPropertyChanged("ComicPage");
            OnPropertyChanged("UploadedDate");
            OnPropertyChanged("ModifiedDate");
            OnPropertyChanged("IsTranslated");
            OnPropertyChanged("IsHidden");
            OnPropertyChanged("IsFavorite");
            OnPropertyChanged("CoverUrl");
            OnPropertyChanged("ImageUrl");
        }

        public async Task SaveComic()
        {
            if (_tempComic != null)
            {
                await CtDb.UpdateComic(_tempComic);
            }
        }

        public async Task UpdateComic()
        {
            if (_tempComic != null)
            {
                Loading = false;
                _tempComic = await CtWebService.searchWorkInfoFromWeb(_tempComic.Workid);
                OnAllPropertyChanged();
                Loading = true;
            }
        }

        public async Task FixComic()
        {
            if (_tempComic != null)
            {
                Loading = false;
                await CtDb.FixComicAsync(_tempComic, _webif);
                OnAllPropertyChanged();
                Loading = true;
            }
        }

        public async void ClearComic()
        {
            Loading = false;
            await Task.Run(() =>
             {
                 _tempComic.Dispose();
                 _tempComic = (CtComic)_oldComic.Clone();
                 OnAllPropertyChanged();
             });
            Loading = true;
        }

        public void DownloadComic()
        {
            if (_tempComic.ImageUrl.Length == 0)
                CtWebService.GetImageUrlFromHitomi(_tempComic);
            CtDownloader ctDownload = new CtDownloader(_tempComic);            
            ctDownload.DownloadWork(false);
        }

        public async Task<List<CtSuggestionItem>> GetSuggestionItemsAsync(string key)
        {
            List<CtSuggestionItem> suggestionItems = new List<CtSuggestionItem>();
            if (key.Length > 0)
            {
                return suggestionItems = await RootPage.CtDb.GetSuggestionItemsAsync(key, CtModelType.ALL);
            }
            else
                return null;
        }

        public void AddTag(CtModel model)
        {
            if (model != null)
            {
                _tempComic.AddModel(model);                
                OnAllPropertyChanged();
            }
        }

        public async Task<ImageSource> DownloadImageAsync()
        {
            try
            {
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, _tempComic.CoverUrl))
                {
                    using (var httpResponse = await _httpClient.SendAsync(requestMessage))
                    {
                        if (httpResponse.StatusCode == HttpStatusCode.OK)
                        {
                            byte[] byteArray = await httpResponse.Content.ReadAsByteArrayAsync();

                            Stream stream = new MemoryStream(byteArray);
                            return ImageSource.FromStream(() => { return stream; });
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

    }
}
