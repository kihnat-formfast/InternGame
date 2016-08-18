using SaveTheIntern.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using System.Threading;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SaveTheIntern
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        DispatcherTimer rebeccaTimer = new DispatcherTimer();
        DispatcherTimer joshTimer = new DispatcherTimer();
        bool humanCaptured = false;
        int mainScore = 0;
        int sideScore = 0;
        int testScore = 0;
        int tomscore = 0;
        int rebScore = 0;
        int irfanScore = 0;
        int ryanScore = 0;

        #region Navigation Helper

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }
        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        /// 
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
#endregion

        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            human.Visibility = Visibility.Collapsed;
            target.Visibility = Visibility.Collapsed;

            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(1.5);

            rebeccaTimer.Tick += rebeccaTimer_Tick;
            rebeccaTimer.Interval = TimeSpan.FromSeconds(10);

            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);

            joshTimer.Tick += joshTimer_Tick;
            joshTimer.Interval = TimeSpan.FromSeconds(20);

            gameOver.Visibility = Visibility.Collapsed;
            gameOverText.Visibility = Visibility.Collapsed;
            playArea.Visibility = Visibility.Collapsed;
        }

        # region Basic Actions

        private async Task StartGame()
        {
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            instructionsButton.Visibility = Visibility.Collapsed;
            human.Visibility = Visibility.Visible;
            target.Visibility = Visibility.Visible;
            playArea.Visibility = Visibility.Visible;
            startPage.Visibility = Visibility.Collapsed;
            gameOver.Visibility = Visibility.Collapsed;
            gameOverText.Visibility = Visibility.Collapsed;

            tomscore = 0;
            rebScore = 0;
            irfanScore = 0;
            ryanScore = 0;
            mainScore = 0;
            sideScore = 0;
            testScore = 0;

            playArea.Children.Clear();
            playArea.Children.Add(target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();
            rebeccaTimer.Start();
            joshTimer.Start();
            Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
            Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
            Canvas.SetLeft(human, random.Next(100, (int)playArea.ActualWidth - 100));
            Canvas.SetTop(human, random.Next(100, (int)playArea.ActualHeight - 100));
        }

        private async Task EndTheGame(string reason)
        {
            if (gameOver.Visibility == Visibility.Collapsed)
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                rebeccaTimer.Stop();
                if (joshTimer.IsEnabled)
                { joshTimer.Stop(); }
                humanCaptured = false;

                startButton.Visibility = Visibility.Visible;
                instructionsButton.Visibility = Visibility.Visible;
                human.Visibility = Visibility.Collapsed;
                target.Visibility = Visibility.Collapsed;
                startText.Visibility = Visibility.Collapsed;
                var list = playArea.Children;
                var person = from obj in list
                          where obj is ContentControl
                          select obj;
                foreach (ContentControl per in person)
                { per.Visibility = Visibility.Collapsed; }
                gameOver.Visibility = Visibility.Visible;
                gameOverText.Visibility = Visibility.Visible;

                int score = (mainScore * 10) + (sideScore * 5) + testScore + tomscore + rebScore + irfanScore + ryanScore;
                scoreValue.Text = score.ToString();
                mainProjectsValue.Text = mainScore.ToString();
                sideProjectsValue.Text = sideScore.ToString();
                testsValue.Text = testScore.ToString();
                tomValue.Text = tomscore.ToString();
                rebeccaValue.Text = rebScore.ToString();
                irfanValue.Text = irfanScore.ToString();
                ryanValue.Text = ryanScore.ToString();
                firedReason.Text = reason;

            }
        }

#endregion

        #region Timers

        void targetTimer_Tick(object sender, object e)
        {
            progressBar.Value += 1;
            if (progressBar.Value >= progressBar.Maximum)
            {EndTheGame("You were fired for running out of time");}
            var list = playArea.Children;
            var reb = from obj in list
                          where obj is ContentControl
                          where ((ContentControl)obj).Template == Resources["RebeccaTemplate"] as ControlTemplate
                          select obj;
            ContentControl rebecca = (ContentControl)reb.FirstOrDefault();
            var joshList = from obj in list
                      where obj is ContentControl
                      where ((ContentControl)obj).Template == Resources["RebeccaTemplate"] as ControlTemplate
                      select obj;
            ContentControl josh = (ContentControl)joshList.FirstOrDefault();
            var tomList = from obj in list
                          where obj is ContentControl
                          where ((ContentControl)obj).Template == Resources["EnemyTemplate"] as ControlTemplate
                          select obj;
            var projectList = from obj in list
                          where obj is ContentControl
                          where ((ContentControl)obj).Template == Resources["ProjectTemplate"] as ControlTemplate
                          select obj;
            var ryanList = from obj in list
                          where obj is ContentControl
                          where ((ContentControl)obj).Template == Resources["RyanTemplate"] as ControlTemplate
                          select obj;
            ContentControl ryan = (ContentControl)ryanList.FirstOrDefault();
            var irfanList = from obj in list
                           where obj is ContentControl
                           where ((ContentControl)obj).Template == Resources["IrfanTemplate"] as ControlTemplate
                           select obj;
            ContentControl irfan = (ContentControl)irfanList.FirstOrDefault();
            var joshableList = from obj in list
                               where obj is ContentControl
                               where ((ContentControl)obj).Template == Resources["TestTemplate"] as ControlTemplate
                               select obj;
            (joshableList.ToList()).Union(projectList.ToList());
            (joshableList.ToList()).Union(ryanList.ToList());
            (joshableList.ToList()).Union(irfanList.ToList());
            if (rebecca != null && (ContentControl)tomList.FirstOrDefault() != null)
            {
                foreach (ContentControl tom in tomList)
                {
                    Point tomCoord = (tom.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    Point rebeccaCoord = (rebecca.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    double distance = Math.Sqrt(Math.Pow(((double)rebeccaCoord.X - (double)tomCoord.X), 2.0) + Math.Pow(((double)rebeccaCoord.Y - (double)tomCoord.Y), 2.0));
                    if (distance < 100)
                    { 
                        playArea.Children.Remove(tom);
                        tomscore += 1;
                        if (ryan == null)
                        { AddRyan(); }
                    }
                }
            }
            if (irfan != null && (ContentControl)projectList.FirstOrDefault() != null)
            {
                foreach (ContentControl project in projectList)
                {
                    Point projCoord = (project.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    Point irfanCoord = (irfan.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    double distance = Math.Sqrt(Math.Pow(((double)irfanCoord.X - (double)projCoord.X), 2.0) + Math.Pow(((double)irfanCoord.Y - (double)projCoord.Y), 2.0));
                    if (distance < 50)
                    {
                        Canvas.SetLeft(project, random.Next(100, (int)playArea.ActualWidth - 100));
                        Canvas.SetTop(project, random.Next(100, (int)playArea.ActualHeight - 100));
                        Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
                        Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                    }
                }
            }
            if (josh != null && (ContentControl)joshableList.FirstOrDefault() != null)
            {
                foreach (ContentControl obj in joshableList)
                {
                    Point objCoord = (obj.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    Point joshCoord = (josh.TransformToVisual(Window.Current.Content)).TransformPoint(new Point(0, 0));
                    double distance = Math.Sqrt(Math.Pow(((double)joshCoord.X - (double)objCoord.X), 2.0) + Math.Pow(((double)joshCoord.Y - (double)objCoord.Y), 2.0));
                    if (distance < 50)
                    {
                        playArea.Children.Remove(obj);
                        AddTest();
                    }
                }
            }
        }

        void enemyTimer_Tick(object sender, object e)
        {
            AddEnemy();
        }

        void rebeccaTimer_Tick(object sender, object e)
        {
            AddRebecca();
        }

        void joshTimer_Tick(object sender, object e)
        {
            AddJosh();
            joshTimer.Stop();
        }

#endregion

        #region Adding Objects

        private void AddProject()
        {
            ContentControl project = new ContentControl();
            project.Template = Resources["ProjectTemplate"] as ControlTemplate;
            Canvas.SetLeft(project, random.Next(100, (int)playArea.ActualWidth - 100));
            Canvas.SetTop(project, random.Next(100, (int)playArea.ActualHeight - 100));
            playArea.Children.Add(project);
            project.PointerEntered += project_PointerEntered;
        }

        private void AddTest()
        {
            ContentControl test = new ContentControl();
            test.Template = Resources["TestTemplate"] as ControlTemplate;
            AnimateIrfan(test, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateIrfan(test, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(test);
            test.PointerEntered += test_PointerEntered;
        }

        private void AddEnemy()
        {
            ContentControl enemy = new ContentControl();
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;
            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(enemy);

            enemy.PointerEntered += enemy_PointerEntered;
        }

        private async Task AddRebecca()
        {
            ContentControl rebecca = new ContentControl();
            int speed = random.Next(6, 10);
            rebecca.Template = Resources["RebeccaTemplate"] as ControlTemplate;
            AnimateRebecca(rebecca, 0, playArea.ActualWidth - 100, "(Canvas.Left)", speed);
            AnimateRebecca(rebecca, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)", speed);
            playArea.Children.Add(rebecca);
            rebecca.PointerEntered += rebecca_PointerEntered;
            await RemoveRebecca(rebecca, speed);
        }

        private async Task AddRyan()
        {
            ContentControl ryan = new ContentControl();
            ryan.Template = Resources["RyanTemplate"] as ControlTemplate;
            AnimateRyan(ryan, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateRyan(ryan, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(ryan);
            ryan.PointerEntered += ryan_PointerEntered;
            await RemoveRyan(ryan);
        }

        private void AddIrfan()
        {
            ContentControl irfan = new ContentControl();
            irfan.Template = Resources["IrfanTemplate"] as ControlTemplate;
            AnimateIrfan(irfan, playArea.ActualWidth - 100, 0, "(Canvas.Left)");
            AnimateIrfan(irfan, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(irfan);
            irfan.PointerEntered += irfan_PointerEntered;
        }

        private void AddJosh()
        {
            ContentControl josh = new ContentControl();
            josh.Template = Resources["JoshTemplate"] as ControlTemplate;
            AnimateIrfan(josh, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateIrfan(josh, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(josh);
            josh.PointerEntered += josh_PointerEntered;
        }

#endregion

        #region Removing Objects

        private async Task RemoveRebecca(ContentControl rebecca, int seconds)
        {
            int time = seconds * 1000;
            await Task.Delay(time);
            playArea.Children.Remove(rebecca);
            rebScore += 1;
        }

        private async Task RemoveRyan(ContentControl ryan)
        {
            await Task.Delay(5000);
            playArea.Children.Remove(ryan);
        }

#endregion

        #region Animating objects

        private void AnimateEnemy(ContentControl contentObject, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)))
            };
            Storyboard.SetTarget(animation, contentObject);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void AnimateIrfan(ContentControl contentObject, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };
            Storyboard.SetTarget(animation, contentObject);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void AnimateRebecca(ContentControl contentObject, double from, double to, string propertyToAnimate, int speed)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = false};
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(speed))
            };
            Storyboard.SetTarget(animation, contentObject);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void AnimateRyan(ContentControl contentObject, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = false };
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(5))
            };
            Storyboard.SetTarget(animation, contentObject);
            Storyboard.SetTargetProperty(animation, propertyToAnimate);
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        #endregion

        # region Events

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void instructionsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Instructions), null);
        }

        void enemy_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
                EndTheGame("You were fired for getting caught by Bossman");
        }

        void rebecca_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
                EndTheGame("You were fired for getting caught by Bosswoman");
        }

        void ryan_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                AddProject();
                var list = playArea.Children;
                var ryanList = from obj in list
                               where obj is ContentControl
                               where ((ContentControl)obj).Template == Resources["RyanTemplate"] as ControlTemplate
                               select obj;
                ContentControl ryan = (ContentControl)ryanList.FirstOrDefault();
                playArea.Children.Remove(ryan);
                var irfanList = from obj in list
                               where obj is ContentControl
                               where ((ContentControl)obj).Template == Resources["IrfanTemplate"] as ControlTemplate
                               select obj;
                ContentControl irfan = (ContentControl)irfanList.FirstOrDefault();
                if (irfan == null)
                { AddIrfan(); }
                ryanScore += 1;
            }
        }

        private void human_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible = false;
            }
        }

        private void irfan_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                if (progressBar.Value > 15)
                { progressBar.Value -= 15; }
                else { progressBar.Value = 0; }
                var list = playArea.Children;
                var irfanList = from obj in list
                                where obj is ContentControl
                                where ((ContentControl)obj).Template == Resources["IrfanTemplate"] as ControlTemplate
                                select obj;
                ContentControl irfan = (ContentControl)irfanList.FirstOrDefault();
                playArea.Children.Remove(irfan);
                irfanScore += 1;
            }

        }

        private void target_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                if (progressBar.Value > 15)
                { progressBar.Value -= 15; }
                else { progressBar.Value = 0; }
                Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                mainScore += 1;
            }
        }

        private void project_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ContentControl project = sender as ContentControl;
            if (humanCaptured)
            {
                if (progressBar.Value > 20)
                { progressBar.Value -= 20; }
                else { progressBar.Value = 0; }
                playArea.Children.Remove(project);
                sideScore += 1;
            }
        }

        private void test_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ContentControl project = sender as ContentControl;
            if (humanCaptured)
            {
                int time = random.Next(-10, 20);
                if (progressBar.Value > time)
                { progressBar.Value -= time; }
                else { progressBar.Value = 0; }
                playArea.Children.Remove(project);
                testScore += 1;
            }
        }

        private void josh_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                AddTest();
            }
        }

        private void playArea_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetCurrentPoint(null).Position;
                Point relativePosition = grid.TransformToVisual(playArea).TransformPoint(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArea_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            if (humanCaptured)
                EndTheGame("You were fired for leaving the office");
        }
        #endregion




    }
}
