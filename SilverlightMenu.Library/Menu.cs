using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SilverlightMenu.Library
{
    public class Menu : Grid
    {
        #region attributes

        public readonly DependencyProperty MenuItemProperty;
        public readonly DependencyProperty ImagesPathProperty;
        public readonly DependencyProperty CommandProperty;
        public readonly DependencyProperty BorderBrushProperty;
        public readonly DependencyProperty TopPanelBrushProperty;
        public readonly DependencyProperty ImageBackgroundBrushProperty;
        public readonly DependencyProperty FocusBrushProperty;
        public readonly DependencyProperty FocusBorderBrushProperty;
        public readonly DependencyProperty ForegroundProperty;
        public new readonly DependencyProperty BackgroundProperty;

        private readonly StackPanel _stackPanel = new StackPanel();
        private readonly Dictionary<string, MenuItem> _menuDictionary = new Dictionary<string, MenuItem>();

        private Brush _borderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x9E, 0xA9, 0xBD));
        private Brush _topPanelBrush;
        private Brush _imageBackgroundBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0xE1, 0xF0));
        private Brush _focusBrush;
        private Brush _focusBorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xC3, 0x65));
        private Brush _foreground = new SolidColorBrush(Colors.Black);
        private Brush _background = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0xD1, 0xE0));
        private const double VerticalMenuWidth = 300;
        private int _selectedMenuLevel = 0;
        private string _imagesPath = "Images/";
        private ICommand _command;

        public event EventHandler MenuItemClicked;

        private bool _isOverMenu = false;

        #endregion attributes

        #region constructor

        public Menu()
        {
            _menuDictionary.Clear();
            SetValue(Canvas.ZIndexProperty, 1000);
            MenuItemProperty =
            DependencyProperty.Register("MenuItem", typeof(MenuItem), typeof(Menu),
            new PropertyMetadata(null,
            OnMenuItemChanged));

            ImagesPathProperty =
            DependencyProperty.Register("ImagesPath", typeof(string), typeof(Menu),
            new PropertyMetadata(null,
            OnImagesPathChanged));

            CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(Menu),
            new PropertyMetadata(null,
            OnCommandChanged));

            BorderBrushProperty =
            DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnBorderBrushChanged));

            TopPanelBrushProperty =
            DependencyProperty.Register("TopPanelBrush", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnTopPanelBrushChanged));

            ImageBackgroundBrushProperty =
            DependencyProperty.Register("ImageBackgroundBrush", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnImageBackgroundBrushChanged));

            FocusBrushProperty =
            DependencyProperty.Register("FocusBrush", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnFocusBrushChanged));

            FocusBorderBrushProperty =
            DependencyProperty.Register("FocusBorderBrush", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnFocusBorderBrushChanged));

            ForegroundProperty =
            DependencyProperty.Register("Foreground", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnForegroundChanged));

            BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Menu),
            new PropertyMetadata(null,
            OnBackgroundChanged));

            var lgb = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };

            lgb.GradientStops.Add(new GradientStop { Offset = 0.0, Color = Color.FromArgb(0xFF, 0xC8, 0xD1, 0xE0) });
            lgb.GradientStops.Add(new GradientStop { Offset = 1.0, Color = Color.FromArgb(0xFF, 0x9E, 0xA9, 0xBD) });

            _topPanelBrush = lgb;

            var lgb2 = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };

            lgb2.GradientStops.Add(new GradientStop { Offset = 0.0, Color = Color.FromArgb(0xFF, 0xFF, 0xFB, 0xF1) });
            lgb2.GradientStops.Add(new GradientStop { Offset = 1.0, Color = Color.FromArgb(0xFF, 0xFF, 0xDC, 0xA5) });

            _focusBrush = lgb2;

            _stackPanel.Orientation = Orientation.Horizontal;
            _stackPanel.Background = _topPanelBrush;
            _stackPanel.Height = 22;
            _stackPanel.VerticalAlignment = VerticalAlignment.Top;
            _stackPanel.SetValue(Canvas.ZIndexProperty, -5000);
            Children.Add(_stackPanel);

            MouseEnter += (s1, e1) =>
            {
                _isOverMenu = true;
            };

            MouseLeave += (s2, e2) =>
            {
                _isOverMenu = false;
            };

            Loaded += (s3, e3) =>
            {
                var parent = (FrameworkElement)Parent;

                parent.MouseLeftButtonUp += (s4, e4) =>
                {
                    if (!_isOverMenu)
                    {
                        foreach (var md in _menuDictionary)
                        {
                            if (md.Value.MenuGrid != null)
                                md.Value.MenuGrid.Visibility = Visibility.Collapsed;
                        }
                        _selectedMenuLevel = 0;
                    }
                };
            };
        }

        #endregion constructor

        #region Methods

        public void ClearReferences(MenuItem menuItem)
        {
            foreach (var childMenuItem in menuItem.MenuItems)
            {
                ClearReferences(childMenuItem);
            }
            if (menuItem.MenuGrid != null)
            {
                menuItem.MenuGrid.Tag = null;
                menuItem.MenuGrid = null;
            }
        }

        public void AddMenuItem(MenuItem parent, MenuItem child)
        {
            _menuDictionary["mnuRecentFiles"].MenuItems.Add(child);
            _stackPanel.Children.Clear();
            ClearReferences(MenuItem);
            _menuDictionary.Clear();
            DrawMenu(this, MenuItem);
        }

        private void DrawMenu(Menu menu, MenuItem menuItem)
        {
            if (menuItem == null)
                return;

            double currentLeftMargin = 0;
            foreach (var mi2 in menuItem.MenuItems)
            {
                mi2.ParentName = menuItem.Name;
                mi2.Level = menuItem.Level + 1;

                const double currentTopMargin = 0;
                var textBlockLevel1 = new TextBlock
                {
                    Text = mi2.Text,
                    Margin = new Thickness(8, 4, 8, 4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = _foreground,
                    FontWeight = FontWeights.Bold
                };

                //var fill = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                //fill.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = Color.FromArgb(0xFF, 0xFF, 0xFB, 0xF1) });
                //fill.GradientStops.Add(new GradientStop() { Offset = 1.0, Color = Color.FromArgb(0xFF, 0xFF, 0xDC, 0xA5) });

                var focusRectangle1 = new Rectangle
                {
                    Fill = _focusBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(2, 2, 2, 2),
                    RadiusX = 2,
                    RadiusY = 2
                };

                var focusRectangle2 = new Rectangle
                {
                    Stroke = _focusBorderBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(2, 2, 2, 2),
                    RadiusX = 2,
                    RadiusY = 2
                };

                var gridLevel1 = new Grid();

                gridLevel1.Children.Add(focusRectangle1);
                gridLevel1.Children.Add(textBlockLevel1);
                gridLevel1.Children.Add(focusRectangle2);
                gridLevel1.Tag = mi2.Name;

                gridLevel1.MouseEnter += grid_MouseEnter;
                gridLevel1.MouseLeave += grid_MouseLeave;
                gridLevel1.MouseLeftButtonUp += gridLevel1_MouseLeftButtonUp;

                menu._stackPanel.Children.Add(gridLevel1);

                //LinearGradientBrush bkg = new LinearGradientBrush() { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
                //bkg.GradientStops.Add(new GradientStop() { Offset = 0.0, Color = Color.FromArgb(0xFF, 0xD8, 0xE1, 0xF0) });

                var txt = new TextBlock
                {
                    Text = mi2.Text,
                    Margin = new Thickness(7, 1, 8, 4),
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = _foreground,
                    FontWeight = FontWeights.Bold
                };

                var brd = new Border
                {
                    CornerRadius = new CornerRadius(2),
                    BorderBrush = _borderBrush,
                    BorderThickness = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                Grid grdVertical = null;

                //grdVertical = CreateVerticalGrid(menu, currentLeftMargin, currentTopMargin, mi2, focusBrush, txt, grdVertical);
                grdVertical = CreateVerticalGrid(menu, currentLeftMargin, currentTopMargin, mi2, txt, grdVertical);

                currentLeftMargin += textBlockLevel1.ActualWidth + 16;
            }
        }

        private Grid CreateVerticalGrid(
        Grid parentGrid,
        double currentLeftMargin,
        double currentTopMargin,
        MenuItem parentMenuItem,
        TextBlock txt,
        Grid grdVertical)
        {
            var verticalGrid = new Grid
            {
                Width = VerticalMenuWidth,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            grdVertical = new Grid
            {
                Margin = new Thickness(currentLeftMargin, currentTopMargin, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            grdVertical.RowDefinitions.Add(new RowDefinition { Height = new GridLength(22) });
            grdVertical.RowDefinitions.Add(new RowDefinition { Height = new GridLength(22) });

            parentGrid.Children.Add(grdVertical);

            parentMenuItem.MenuGrid = grdVertical;

            var brdAroundVerticalParent = new Border
            {
                Margin = new Thickness(0, 1, 0, 0),
                CornerRadius = new CornerRadius(2),
                BorderBrush = _borderBrush,
                BorderThickness = new Thickness(1),
                Background = _background,
                Height = 22,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            brdAroundVerticalParent.RenderTransform = new TranslateTransform { Y = 0 };
            brdAroundVerticalParent.SetValue(RowProperty, 0);
            brdAroundVerticalParent.SetValue(Canvas.ZIndexProperty, parentMenuItem.Level * 100);

            if (parentMenuItem.Level == 1)
                brdAroundVerticalParent.Child = txt;

            if (parentMenuItem.Level > 1)
                brdAroundVerticalParent.Visibility = Visibility.Collapsed;

            brdAroundVerticalParent.MouseLeftButtonUp += brdAroundVerticalParent_MouseLeftButtonUp;

            grdVertical.Children.Add(brdAroundVerticalParent);

            var brdAroundVerticalChildren = new Border
            {
                CornerRadius = new CornerRadius(2),
                BorderBrush = _borderBrush,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            brdAroundVerticalChildren.Child = verticalGrid;

            brdAroundVerticalChildren.RenderTransform = new TranslateTransform { Y = 0 };
            brdAroundVerticalChildren.SetValue(RowProperty, 1);

            grdVertical.Children.Add(brdAroundVerticalChildren);
            var rectHideLine = new Rectangle
            {
                Margin = new Thickness(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Stroke = _background,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 1
            };
            rectHideLine.SetValue(RowProperty, 1);
            var rectLineLeft = new Rectangle
            {
                Margin = new Thickness(0, 5, 0, 0),
                Fill = _borderBrush,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 1,
                Height = 22
            };

            rectLineLeft.SetValue(RowProperty, 0);
            rectLineLeft.SetValue(RowSpanProperty, 2);

            if (parentMenuItem.Level < 2)
            {
                grdVertical.Children.Add(rectHideLine);
                grdVertical.Children.Add(rectLineLeft);
            }

            var gridRow = 0;
            foreach (var mi3 in parentMenuItem.MenuItems)
            {
                mi3.ParentName = parentMenuItem.Name;
                mi3.Level = parentMenuItem.Level + 1;

                var focusRectangle3 = new Rectangle
                {
                    Fill = _focusBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(1, 0, 10, 0),
                    RadiusX = 2,
                    RadiusY = 2
                };

                var focusRectangle4 = new Rectangle
                {
                    Stroke = _focusBorderBrush,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Visibility = Visibility.Collapsed,
                    Margin = new Thickness(1, 0, 10, 0),
                    RadiusX = 2,
                    RadiusY = 2
                };

                var separatorRectangle = new Rectangle
                {
                    Height = 1,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0)),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(2, 0, 2, 0)
                };
                separatorRectangle.SetValue(ColumnProperty, 1);
                separatorRectangle.SetValue(ColumnSpanProperty, 2);

                var brdImage = new Border
                {
                    Width = 24,
                    Height = 22,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Background = _imageBackgroundBrush
                };
                var imgLevel2 = new Image
                {
                    Width = 16,
                    Height = 16,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(4, 0, 0, 0),
                    Opacity = mi3.IsEnabled ? 1.0 : 0.5
                };
                imgLevel2.SetValue(ColumnProperty, 0);

                var imgArrow = new Image
                {
                    Width = 16,
                    Height = 16,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(0, 0, 0, 0),
                    Opacity = mi3.IsEnabled ? 1.0 : 0.5
                };
                imgArrow.SetValue(ColumnProperty, 2);
                imgArrow.Source = new BitmapImage(new Uri(string.Format(@"{0}Arrow.png", _imagesPath), UriKind.Relative));
                imgArrow.Visibility = mi3.MenuItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

                if (mi3.IsCheckable)
                {
                    if (mi3.IsChecked)
                        mi3.ImagePath = string.Format("{0}Checked.png", _imagesPath);
                    else
                        mi3.ImagePath = null;
                }
                else
                {
                    if (mi3.ImagePath == null)
                        mi3.ImagePath = string.Format("{0}{1}.png", _imagesPath, mi3.Name);
                }

                if (mi3.ImagePath != null)
                {
                    imgLevel2.Source = new BitmapImage(new Uri(mi3.ImagePath, UriKind.Relative));
                }

                var textBlockLevel2 = new TextBlock();

                textBlockLevel2.Text = mi3.Text;
                textBlockLevel2.Margin = new Thickness(8, 4, 8, 4);
                textBlockLevel2.VerticalAlignment = VerticalAlignment.Center;
                textBlockLevel2.Foreground = _foreground;
                textBlockLevel2.Opacity = mi3.IsEnabled ? 1.0 : 0.5;

                textBlockLevel2.FontWeight = FontWeights.Bold;
                ;

                var gridLevel2 = new Grid
                {
                    Background = _background
                };

                gridLevel2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24) });
                gridLevel2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(VerticalMenuWidth - 40) });
                gridLevel2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(24), MinWidth = 24 });

                focusRectangle3.SetValue(ColumnProperty, 0);
                focusRectangle3.SetValue(ColumnSpanProperty, 3);

                textBlockLevel2.SetValue(ColumnProperty, 1);
                focusRectangle4.SetValue(ColumnProperty, 0);
                focusRectangle4.SetValue(ColumnSpanProperty, 3);

                verticalGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(22) });

                if (mi3.Text.Equals("-"))
                {
                    gridLevel2.Children.Add(brdImage);
                    gridLevel2.Children.Add(separatorRectangle);
                    verticalGrid.RowDefinitions[verticalGrid.RowDefinitions.Count - 1].Height = new GridLength(2);
                    separatorRectangle.SetValue(ColumnProperty, 1);
                    separatorRectangle.SetValue(ColumnSpanProperty, 2);
                }
                else
                {
                    gridLevel2.Children.Add(brdImage);
                    gridLevel2.Children.Add(focusRectangle3);
                    gridLevel2.Children.Add(imgLevel2);
                    gridLevel2.Children.Add(textBlockLevel2);
                    gridLevel2.Children.Add(imgArrow);
                    gridLevel2.Children.Add(focusRectangle4);

                    gridLevel2.MouseEnter += gridLevel2_MouseEnter;
                    gridLevel2.MouseLeave += gridLevel2_MouseLeave;
                    gridLevel2.MouseLeftButtonUp += gridLevel2_MouseLeftButtonUp;
                }
                gridLevel2.Tag = mi3;
                gridLevel2.SetValue(RowProperty, gridRow);
                verticalGrid.Children.Add(gridLevel2);

                grdVertical.RowDefinitions[1].Height = new GridLength(grdVertical.RowDefinitions[1].Height.Value + 22);
                gridRow++;

                if (mi3.MenuItems.Count > 0)
                {
                    CreateVerticalGrid(parentGrid, currentLeftMargin + VerticalMenuWidth, currentTopMargin, mi3, txt, grdVertical);
                }

                if (mi3.Text.Equals("-"))
                {
                    currentTopMargin += 2;
                }
                else
                {
                    currentTopMargin += 22;
                }

                if (!_menuDictionary.ContainsKey(mi3.Name))
                    _menuDictionary.Add(mi3.Name, mi3);
            }
            grdVertical.Visibility = Visibility.Collapsed;
            grdVertical.MouseLeave += grdVertical_MouseLeave;
            if (!_menuDictionary.ContainsKey(parentMenuItem.Name))
            {
                _menuDictionary.Add(parentMenuItem.Name, parentMenuItem);
            }
            else
            {
            }

            grdVertical.SetValue(Canvas.ZIndexProperty, parentMenuItem.Level * 100);

            return grdVertical;
        }

        private void HideSiblings(MenuItem menuItem)
        {

            if (menuItem.ParentName != null)
            {
                var parentMenuItem = _menuDictionary[menuItem.ParentName];
                foreach (var siblingMenu in parentMenuItem.MenuItems)
                {
                    if (siblingMenu.Name != menuItem.Name)
                    {
                        if (_menuDictionary.ContainsKey(siblingMenu.Name))
                        {
                            if (_menuDictionary[siblingMenu.Name].MenuGrid != null)
                                _menuDictionary[siblingMenu.Name].MenuGrid.Visibility = Visibility.Collapsed;
                        }

                        foreach (var child in siblingMenu.MenuItems)
                        {
                            HideSiblings(child);
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Properties

        public MenuItem MenuItem
        {
            get { return (MenuItem)GetValue(MenuItemProperty); }
            set { SetValue(MenuItemProperty, value); }
        }

        public string ImagesPath
        {
            get { return (string)GetValue(ImagesPathProperty); }
            set { SetValue(ImagesPathProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        public Brush TopPanelBrush
        {
            get { return (Brush)GetValue(TopPanelBrushProperty); }
            set { SetValue(TopPanelBrushProperty, value); }
        }

        public Brush ImageBackgroundBrush
        {
            get { return (Brush)GetValue(ImageBackgroundBrushProperty); }
            set { SetValue(ImageBackgroundBrushProperty, value); }
        }

        public Brush FocusBrush
        {
            get { return (Brush)GetValue(FocusBrushProperty); }
            set { SetValue(FocusBrushProperty, value); }
        }

        public Brush FocusBorderBrush
        {
            get { return (Brush)GetValue(FocusBorderBrushProperty); }
            set { SetValue(FocusBorderBrushProperty, value); }
        }

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        public new Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion Properties

        #region Events

        public void OnMenuItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (Menu)d;
            var menuItem = (MenuItem)e.NewValue;

            DrawMenu(menu, menuItem);
        }

        public void OnImagesPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _imagesPath = e.NewValue.ToString();
        }

        public void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _command = (ICommand)e.NewValue;
        }

        public void OnBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _borderBrush = (Brush)e.NewValue;
        }

        public void OnTopPanelBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _topPanelBrush = (Brush)e.NewValue;
            _stackPanel.Background = _topPanelBrush;
        }

        public void OnImageBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _imageBackgroundBrush = (Brush)e.NewValue;
        }

        public void OnFocusBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _focusBrush = (Brush)e.NewValue;
        }

        public void OnFocusBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _focusBorderBrush = (Brush)e.NewValue;
        }

        public void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _foreground = (Brush)e.NewValue;
        }

        public void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _background = (Brush)e.NewValue;
        }

        private void brdAroundVerticalParent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)((Border)sender).Parent;
            grid.Visibility = Visibility.Collapsed;
            _selectedMenuLevel = 0;
        }

        private void gridLevel2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _selectedMenuLevel = 0;
            var grid = (Grid)sender;
            var menuItem = (MenuItem)grid.Tag;
            if (menuItem.IsEnabled)
            {
                foreach (var item in _menuDictionary)
                {
                    if (item.Value.MenuGrid != null)
                        item.Value.MenuGrid.Visibility = Visibility.Collapsed;
                }
            }

            if (menuItem.IsCheckable)
            {
                menuItem.IsChecked = !menuItem.IsChecked;

                var img = (Image)(grid.Children[2]);
                if (menuItem.IsChecked)
                    img.Source = new BitmapImage(new Uri(string.Format(@"{0}Checked.png", _imagesPath), UriKind.Relative));
                else
                    img.Source = null;
            }

            if (MenuItemClicked != null)
                MenuItemClicked(menuItem, new EventArgs());

            if (_command != null)
                _command.Execute(menuItem);
        }

        private void gridLevel2_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
       
            grid.Children[1].Visibility = Visibility.Collapsed;
            grid.Children[5].Visibility = Visibility.Collapsed;
        }

        private void gridLevel2_MouseEnter(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            var menuItem = (MenuItem)grid.Tag;

            if (menuItem.IsEnabled)
            {
                grid.Children[1].Visibility = Visibility.Visible;
                grid.Children[5].Visibility = Visibility.Visible;
            }

            if (menuItem.MenuGrid != null)
            {
                menuItem.MenuGrid.Visibility = Visibility.Visible;
            }

            HideSiblings(menuItem);
        }

        private void gridLevel1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var grid = (Grid)sender;
            if (_menuDictionary[grid.Tag.ToString()].MenuGrid.Visibility == Visibility.Collapsed)
            {
                _menuDictionary[grid.Tag.ToString()].MenuGrid.Visibility = Visibility.Visible;
                _selectedMenuLevel = 1;
            }
            else
            {
                _menuDictionary[grid.Tag.ToString()].MenuGrid.Visibility = Visibility.Collapsed;
                _selectedMenuLevel = 0;
            }
        }

        private void grdVertical_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
        }

        private void grid_MouseLeave(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            grid.Children[0].Visibility = Visibility.Collapsed;
            grid.Children[2].Visibility = Visibility.Collapsed;
        }

        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            var grid = (Grid)sender;
            if (_selectedMenuLevel == 0)
            {
                grid.Children[0].Visibility = Visibility.Visible;
                grid.Children[2].Visibility = Visibility.Visible;
            }
            else
            {
                foreach (var item in _menuDictionary)
                {
                    if (item.Value.MenuGrid != null)
                        item.Value.MenuGrid.Visibility = Visibility.Collapsed;
                }
                _menuDictionary[grid.Tag.ToString()].MenuGrid.Visibility = Visibility.Visible;
            }
        }

        #endregion Events
    }
}