using ComicViewer.CtModels;
using ComicViewer.ViewModels;
using SharpCifs.Util.Sharpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComicViewer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ModelDetailPage : ContentPage
	{
        public ModelDetailViewModel viewModel;
        CtModelList fixedModels = new CtModelList();

        public ModelDetailPage (CtModel model)
        {
            InitializeComponent();
            SetModel(model);
        }

        private void SetModel(CtModel model)
        {
            FieldBox.Children.Clear();
            BindingContext = viewModel = new ModelDetailViewModel(model);
            for (int i = 0; i < model.values.Count(); i++)
            {
                if (model.Types[i] == typeof(bool))
                {
                    FieldBox.Children.Add(new Switch() { IsToggled = (bool)model.values[i] });
                }
                else if (model.Types[i] == typeof(string))
                {
                    if (model.CtType == CtModelType.WORD)
                    {
                        if (i < 2)
                        {
                            FieldBox.Children.Add(new Entry()
                            {
                                Placeholder = model.ColunmText[i],
                                Text = (string)model.values[i],
                            });
                        }
                        else
                        {
                            FieldBox.Children.Add(new WordFieldsEntry(i, viewModel.RootPage)
                            {
                                Text = (string)model.values[i],
                            });
                        }
                    }
                    else
                    {
                        FieldBox.Children.Add(new Entry()
                        {
                            Placeholder = model.ColunmText[i],
                            Text = (string)model.values[i],
                        });
                    }
                }
            }

            if (viewModel.Model.CtType == CtModelType.NAME)
            {
                ((Entry)FieldBox.Children[0]).TextChanged += ModelDetailPage_TextChanged;
            }
        }

        protected override async void OnAppearing()
        {
            if (viewModel.Model.CtType == CtModelType.NAME)
            {
                flexBox.Children.Clear();
                foreach (CtCharacter character in await viewModel.GetNameCharacter(viewModel.Model.NameEn))
                {
                    flexBox.Children.Add(new Label() { Text = character.NameEn, Margin = new Thickness(5, 0) });
                }
            }
            if(viewModel.Model.CtType == CtModelType.WORD) 
                webView.Source = new UrlWebViewSource() { Url = "https://translate.google.co.kr/?hl=ko#view=home&op=translate&sl=ja&tl=ko&text=" + viewModel.Model.Name.Replace(" ", "%20") };
            else
                webView.Source = new UrlWebViewSource() { Url = "https://www.google.co.kr/webhp?sourceid=chrome&ie=UTF-8#q=" + viewModel.Model.Name.Replace(' ', '+') };
            base.OnAppearing();
        }

        private async void ModelDetailPage_TextChanged(object sender, TextChangedEventArgs e)
        {
            flexBox.Children.Clear();
            foreach (CtCharacter character in await viewModel.GetNameCharacter(e.NewTextValue))
            {
                flexBox.Children.Add(new Label() { Text = character.NameEn, Margin = new Thickness(5, 0) });
            }
        }

        public async Task<string> TransWebBrowserInitAsync(string text, string from, string to)
        {
            return await webView.TransWebBrowserInitAsync(text, from, to);
        }

        public async Task<string> GetWebClintContentsAsync(string url)
        {
            return await webView.GetWebClintContentsAsync(url);
        }

        private async void SearchItem_Clicked(object sender, EventArgs e)
        {
            string res = await GetWebClintContentsAsync("https://myanimelist.net/search/all?q=" + viewModel.Model.NameEn.Replace(" ", "%20"));
            res = Library.extractionString(res, "<div class=\"content-result\">", "<div class=\"content-right\">");
            res = Library.extractionString(res, "<article>", "</article>");
            res = Library.extractionString(res, "href=\"", "\"");
            if(res.Replace("_","").ToLower().Contains(viewModel.Model.NameEn.Replace(" ", "").ToLower()))
                res = await GetWebClintContentsAsync(res);
        }

        private async void SaveItem_Clicked(object sender, EventArgs e)
        {
            bool res = await viewModel.RootPage.DisplayAlert("알림", $"{fixedModels.Count - fixedModels.Counts[(int)CtModelType.COMIC]}개의 태그와 {fixedModels.Counts[(int)CtModelType.COMIC]}개의 작품을 저장하시겠습니까?", "예", "아니오");

            if (res)
            {
                await viewModel.SaveModels(fixedModels);
            }
        }
        
        private async void LoadItem_Clicked(object sender, EventArgs e)
        {
            if(FieldBox.Children.Count > 0 && ((Entry)FieldBox.Children[0]).Text.Length > 0)
            {
                string id = ((Entry)FieldBox.Children[0]).Text;
                CtModel model = await viewModel.CtDb.GetModel(viewModel.Model.CtType, id);
                if(model != null)
                    SetModel(model);
            }
        }

        private void ClearItem_Clicked(object sender, EventArgs e)
        {
            for (int i = 0; i < viewModel.OldModel.values.Count(); i++)
            {
                if (viewModel.OldModel.Types[i] == typeof(bool))
                {
                    ((Switch)FieldBox.Children[i]).IsToggled = (bool)viewModel.OldModel.values[i];
                }
                else if (FieldBox.Children[i].GetType() == typeof(Entry))
                {
                    ((Entry)FieldBox.Children[i]).Text = (string)viewModel.OldModel.values[i];
                }
                else if (FieldBox.Children[i].GetType() == typeof(WordFieldsEntry))
                {
                    ((WordFieldsEntry)FieldBox.Children[i]).Text = (string)viewModel.OldModel.values[i];
                }
            }
        }
        
        private async void ApplyItem_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Model == null) return;

            fixedModels.Clear();
            for (int i = 0; i < FieldBox.Children.Count(); i++)
            {
                if (viewModel.Model.Types[i] == typeof(bool))
                {
                    viewModel.Model.values[i] = ((Switch)FieldBox.Children[i]).IsToggled;
                }
                else if (FieldBox.Children[i].GetType() == typeof(Entry))
                {
                    viewModel.Model.values[i] = ((Entry)FieldBox.Children[i]).Text;
                }
                else if (FieldBox.Children[i].GetType() == typeof(WordFieldsEntry))
                {
                    viewModel.Model.values[i] = ((WordFieldsEntry)FieldBox.Children[i]).Text;
                }
            }

            if (viewModel.Model.CtType == CtModelType.NAME)
            {
                CtName newName = viewModel.Model.Clone() as CtName;
                CtNameList newNameList = new CtNameList();

                await Task.Run(async () =>
                {
                    foreach (CtCharacter character in await viewModel.GetNameCharacter(newName.NameEn))
                    {
                        CtCharacter newCharacter = character.Clone() as CtCharacter;
                        CtNameList tempNameList = new CtNameList();
                        tempNameList.AddRange(await viewModel.CtDb.GetNames());
                        tempNameList.UpdateAddModel(newName);
                        newCharacter.TranslateBy(tempNameList, newNameList);
                        fixedModels.Add(newCharacter);
                    }
                    fixedModels.Add(newName);
                    fixedModels.AddRange(await viewModel.FixModel(fixedModels));
                });
            }
            else if (viewModel.Model.CtType == CtModelType.WORD)
            {
                string temp;
                CtWord newword = viewModel.Model.Clone() as CtWord;
                CtWordList words = new CtWordList();
                bool errorFlag = false;

                await Task.Run(async () =>
                {
                    if (newword == null) return;
                    int triggerCount = Library.StringDivider(newword.Divider, "/").Count;
                    if (Library.StringDivider(newword.NameKr, "/").Count != triggerCount + 1)
                        errorFlag = true;
                    else if (Library.StringDivider(newword.Space, "/").Count != triggerCount + 1)
                        errorFlag = true;
                    else if (Library.StringDivider(newword.TypeBck, "/").Count != triggerCount + 1)
                        errorFlag = true;
                    else if (Library.StringDivider(newword.TypeFwd, "/").Count != triggerCount + 1)
                        errorFlag = true;
                    else if (Library.StringDivider(newword.Language, "/").Count != 1 && Library.StringDivider(newword.Language, "/").Count != triggerCount + 1)
                        errorFlag = true;
                    if (!errorFlag)
                    {
                        if (newword.NameEn.Contains(' ') || newword.NameEn.Contains('-'))
                        {
                            temp = newword.NameEn.Replace(" ", string.Empty);
                            if (temp.First() == '-')
                                temp = ' ' + temp.Remove(0, 1);
                            else if (temp.Last() == '-')
                                temp = temp.Remove(temp.Length - 1, 1) + ' ';
                            else
                                temp = temp.Replace("-", string.Empty);
                            temp = temp.ToTitleCase();
                            words.Add(new CtWord(newword.NameEn, temp, "합성어", "", "", "", newword.Language));
                            newword.NameEn = temp.Trim();
                            words.Add(newword);
                        }
                        else
                        {
                            words.Add(newword);
                        }
                        fixedModels.AddRange(await viewModel.SaveWord(words));
                        fixedModels.AddRange(words);
                    }
                });
                if (errorFlag)
                {
                    await viewModel.RootPage.DisplayAlert("알림", "분류에 이상이 있습니다.", null, "확인");
                    return;
                }
            }
            else
            {
                CtModel newModel = viewModel.Model.Clone();
                await Task.Run(async () =>
                {
                    fixedModels.Add(newModel);
                    fixedModels.AddRange(await viewModel.FixModel(fixedModels));
                });
            }
            viewModel.Comics.Clear();
            foreach (CtModel model in fixedModels)
            {
                if(model.CtType == CtModelType.COMIC)
                {
                    CtComic comic = model as CtComic;
                    comic.ModifiedDate = DateTime.Now;
                    comic.IsSelected = true;
                    viewModel.Comics.Add(await viewModel.CtDb.GetComic(comic.Workid));
                    viewModel.Comics.Add(comic);
                }
            }
            viewModel.OnPropertyChanged("Comics");
            webViewView.IsVisible = false;
            comicListViewView.IsVisible = true;
            await viewModel.RootPage.DisplayAlert("알림", $"{fixedModels.Count - fixedModels.Counts[(int)CtModelType.COMIC]}개의 태그와 {fixedModels.Counts[(int)CtModelType.COMIC]}개의 작품이 수정되었습니다.", "확인");
        }

        private async void GoogleItem_Clicked(object sender, EventArgs e)
        {
            await Application.Current.MainPage.Navigation.PushModalAsync(viewModel.RootPage.GoogleTanslatePage);
        }

        private async void ComicListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            CtComic selectedComic = (CtComic)e.Item;
            string[] strs = { "웹 뷰어", "뷰어", "편집", "제목 복사", "표지 보기", selectedComic.IsFavorite ? "즐겨찾기 해제" : "즐겨찾기 추가", selectedComic.IsHidden ? "숨김 해제" : "숨기기" };
            string select = await DisplayActionSheet(selectedComic.Title, null, null, strs);

            if (select == "웹 뷰어")
            {
                viewModel.RootPage.WebViewerPage.Navigate(selectedComic.GetWebViewerUrl());
                //await viewModel.RootPage.NavigateFromMenu(ViewModels.MenuItemType.WebViewer);
                await Navigation.PushModalAsync(viewModel.RootPage.WebViewerPage);
            }
            else if (select == "뷰어")
            {
                //await Navigation.PushModalAsync(new ViewerPage(selectedComic));
            }
            else if (select == "편집")
            {
                await Navigation.PushAsync(new ComicDetailPage(selectedComic));
            }
            else if (select == "제목 복사")
            {
                await Clipboard.SetTextAsync(selectedComic.TitleEn);
            }
            else if (select == "표지 보기")
            {
                viewModel.RootPage.ImagePage.ImageLoad(selectedComic.GetPageImageUrl(0), selectedComic.CoverUrl);
                await Navigation.PushModalAsync(viewModel.RootPage.ImagePage);
            }
            else if (select == "즐겨찾기 해제")
            {
                selectedComic.IsFavorite = false;
                await viewModel.CtDb.UpdateComic(selectedComic);
            }
            else if (select == "즐겨찾기 추가")
            {
                selectedComic.IsFavorite = true;
                await viewModel.CtDb.UpdateComic(selectedComic);
            }
            else if (select == "숨김 해제")
            {
                selectedComic.IsHidden = false;
                await viewModel.CtDb.UpdateComic(selectedComic);
            }
            else if (select == "숨기기")
            {
                selectedComic.IsHidden = true;
                await viewModel.CtDb.UpdateComic(selectedComic);
            }
        }
    }

    public class WordFieldsEntry : StackLayout
    {

        Entry entry = new Entry() { HorizontalOptions = LayoutOptions.FillAndExpand };
        Button button = new Button() { Text = "+", WidthRequest = 30 };
        public string Text { get { return entry.Text; } set { entry.Text = value; } }
        public WordFieldsEntry(int valueIndex, Page page)
        {
            this.Orientation = StackOrientation.Horizontal;
            this.Children.Add(button);
            this.Children.Add(entry);
            if (valueIndex == 2)
            {
                button.Clicked += async (s, e) =>
                {
                    {
                        string[] str = { "지우기", "false", "true", "true/true/true/true/true/true/true/true/true", "false/false/false/false/false/false/false/false/false" };
                        string res = await page.DisplayActionSheet(null, null, null, str);

                        if (res != null && res.Length > 0)
                            entry.Text = (res == "지우기" ? "" : entry.Text.Length == 0 ? res : entry.Text + "/" + res);
                    }
                };
            }
            else if (valueIndex == 3)
            {
                button.Clicked += async (s, e) =>
                {
                    {
                        string[] str = {
                            "지우기",
                            "형용사/형용사/형용사/형용사/형용사/형용사/형용사/형용사/형용사",
                            "동사/동사/동사/동사/동사/동사/동사/동사/동사",
                            "명사(사람)",
                            "명사(사물)",
                            "명사(장소)",
                            "명사(시간)",
                            "명사(추상)",
                            "명사(상태)",
                            "명사(행위)",
                            "명사(숫자)",
                            "동사",
                            "형용사",
                            "형용사(사람)",
                            "부사",
                            "접미사",
                            "접속사",
                            };
                        string res = await page.DisplayActionSheet(null, null, null, str);
                        if (res != null && res.Length > 0)
                            entry.Text = (res == "지우기" ? "" : entry.Text.Length == 0 ? res : entry.Text + "/" + res);
                    }
                };
            }
            else if (valueIndex == 4)
            {
                button.Clicked += async (s, e) =>
                {
                    {
                        string[] str = {
                            "지우기",
                            "종결/접속사/접속사/접속사/접속사/동형용사/선어말/선어말/선어말",
                            "종결/명사(추상)/선어말/선어말/선어말/선어말/동형용사/동형용사/동형용사",
                            "명사(사람)",
                            "명사(사물)",
                            "명사(장소)",
                            "명사(시간)",
                            "명사(추상)",
                            "명사(상태)",
                            "명사(행위)",
                            "명사(숫자)",
                            "형용사",
                            "동형용사",
                            "동사",
                            "선어말",
                            "부사",
                            "조사",
                            "접두사",
                            "접속사",
                            "종결",
                            };
                        string res = await page.DisplayActionSheet(null, null, null, str);
                        if (res != null && res.Length > 0)
                            entry.Text = (res == "지우기" ? "" : entry.Text.Length == 0 ? res : entry.Text + "/" + res);
                    }
                };
            }
            else if (valueIndex == 5)
            {
                button.Clicked += async (s, e) =>
                {
                    {
                        string[] str = {
                            "지우기",
                            "문장끝/@-라고/@-해도/@-해서/@-하고/@-할/@-하지/@-해-",
                            "문장끝/@-하기/@-한다/@-했-/@-해-/@-하-/@-할/@-하는",
                            "사람",
                            "장소",
                            "숫자",
                            "외국",
                            "행위",
                            "상태",
                            "문장끝",
                            "선어말",
                            "문장시작",
                            "뒤에숫자",
                            "뒤에동사",
                            "보조동사",
                            "사역행위",
                            };
                        string res = await page.DisplayActionSheet(null, null, null, str);
                        if (res != null && res.Length > 0)
                            entry.Text = (res == "지우기" ? "" : entry.Text.Length == 0 ? res : entry.Text + "/" + res);
                    }
                };
            }
            else if (valueIndex == 6)
            {
                button.Clicked += async (s, e) =>
                {
                    {
                        string[] str = {
                            "지우기",
                            "일본",
                            "외국",
                            "절대외국"};
                        string res = await page.DisplayActionSheet(null, null, null, str);
                        if (res != null && res.Length > 0)
                            entry.Text = (res == "지우기" ? "" : res);
                    }
                };
            }
        }
    }
}