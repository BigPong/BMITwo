using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>

    public partial class MainWindow : Window
    {
        // Timer很棒
        System.Windows.Threading.DispatcherTimer t = new System.Windows.Threading.DispatcherTimer();

        // 宣告儲存List
        List<bTextBlock> obj = new List<bTextBlock>();

        // 初始化Timer
        public MainWindow()
        {
            InitializeComponent();
            t.Interval = new TimeSpan(10000);
            t.Tick += new EventHandler(t_Tick);
            t.Start();
        }

        // Draw()
        void t_Tick(object sender, EventArgs e)
        {
            // 如果有多餘一個的答案
            if (obj.Count > 1)
            {
                // 讓目前顯示的被砍掉
                bTextBlock o = obj[0];
                DrawCanvas.Children.Remove(o.rect);
                o.Die();
                DrawCanvas.Children.Add(o.rect);

                // 砍掉到剩最新的
                if (o.ellipSize < 0.1)
                {
                    for (int i = 0; i < obj.Count - 1; i++)
                    {
                        DrawCanvas.Children.Remove(obj[i]);
                    }
                    obj.RemoveRange(0, obj.Count - 1);
                }
            }

            // 如果只有一個就直接顯示不用管
            else if (obj.Count > 0)
            {
                // 更新Rect
                bTextBlock o = obj[0];
                DrawCanvas.Children.Remove(o.rect);
                o.Draw();
                DrawCanvas.Children.Add(o.rect);

                if (obj[0].BMI < 18.5)
                {
                    ansBox.Text = "過    輕";

                    // 判斷免兵役
                    militaryBox.Text = "";
                    if (obj[0].BMI < 16.5)
                    {
                        militaryBox.Text = "免    兵    役";
                    }
                }
                else if (obj[0].BMI < 24)
                {
                    ansBox.Text = "正    常";
                    militaryBox.Text = "";
                }
                else
                {
                    ansBox.Text = "過    重";

                    // 判斷免兵役
                    militaryBox.Text = "";
                    if (obj[0].BMI > 31.5)
                    {
                        militaryBox.Text = "免    兵    役";
                    }
                }
            }
        }

        // 計算BMI
        private void caculater_BMI(object sender, RoutedEventArgs e)
        {
            // 計算
            double height = Convert.ToDouble(Height.Text) / 100;
            double weight = Convert.ToDouble(Weight.Text);
            double ans = weight / Math.Pow(height, 2);

            // 創建TextBlock
            bTextBlock TB = new bTextBlock(175, ans);
            DrawCanvas.Children.Add(TB);
            obj.Add(TB);
            TB.Arrange(new Rect(0, 0, TB.DesiredSize.Width, TB.DesiredSize.Height));
            TB.x -= TB.ActualWidth;
        }

        class bTextBlock : TextBlock
        {
            // 宣告參數
            public Ellipse rect;
            public double BMI;
            double xspeed = 0, yspeed = 0;
            public double x = 0, y = 0;
            public double ellipSize = 0;
            bool Ground = false;
            Color color = new Color();
            public double size = 1;

            // 建構子
            public bTextBlock(double xx, double BMI)
            {
                // 初始化
                this.BMI = BMI;
                Text = BMI.ToString("#0.");
                FontSize = 50;
                FontFamily = new FontFamily("Comic Sans MS");
                x = xx;
                Margin = new Thickness(x, 0, 0, 0);
                Panel.SetZIndex(this, 1);
                Visibility = Visibility.Hidden;
            }

            public void Draw()
            {
                // 更新位置
                this.Margin = new Thickness(x, y, 0, 0);
                Visibility = Visibility.Visible;

                // 重力與彈力因重量不同
                if (y + yspeed + FontSize >= 486 - 5)
                {
                    y = 486 - FontSize - 5;
                    Ground = true;
                    if (yspeed > 1)
                    {
                        yspeed *= (-1) * (1 - (BMI * 0.02));
                    }
                    else
                    {
                        yspeed = 0;
                    }
                }
                else
                {
                    y += yspeed;
                    yspeed += BMI * 0.025;
                }

                // 背景顏色  其實不用重複做  應該可以寫得更簡潔
                color = new Color();

                if (BMI < 18.5)
                {
                    color = (Color)ColorConverter.ConvertFromString("#42E1F4");
                }
                else if (BMI < 24)
                {
                    color = (Color)ColorConverter.ConvertFromString("#FFD944");
                }
                else
                {
                    color = (Color)ColorConverter.ConvertFromString("#F4652D");
                }

                SolidColorBrush brush = new SolidColorBrush(color);

                // 彈跳圈圈繪製
                rect = new Ellipse
                {
                    Fill = brush
                };

                // 重新定位圓圈
                rect.Width = ellipSize;
                rect.Height = ellipSize;
                Canvas.SetLeft(rect, 144 - rect.Width / 2);
                Canvas.SetTop(rect, 486 - rect.Height / 2);

                // 碰到地板才開始
                if (Ground)
                {
                    ellipSize += (1200 - ellipSize) * 0.05;
                }
            }

            public void Die()
            {
                // 字縮小 到後面才發現有Scale函數= =
                if (size > 0.01)
                {
                    ScaleTransform scale = new ScaleTransform(size, size, ActualWidth / 2, ActualHeight / 2);
                    this.RenderTransform = scale;
                    size *= 0.7;
                }

                // 圈圈縮小 應該可以寫得更簡潔  我白癡= =
                if (ellipSize > 0.01)
                {
                    ellipSize = ellipSize * 0.7;

                    SolidColorBrush brush = new SolidColorBrush(color);

                    rect = new Ellipse
                    {
                        Fill = brush
                    };

                    // 重新定位圓圈
                    rect.Width = ellipSize;
                    rect.Height = ellipSize;
                    Canvas.SetLeft(rect, 144 - rect.Width / 2);
                    Canvas.SetTop(rect, 486 - rect.Height / 2);
                }
            }
        }
    }
}