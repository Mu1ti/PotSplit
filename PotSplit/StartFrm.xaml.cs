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
using System.Windows.Shapes;

namespace PotSplit
{
    /// <summary>
    /// StartFrm.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class StartFrm : Window
    {
        public StartFrm()
        {
            BitmapSource LoadingImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(Properties.Resources.Loading.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromWidthAndHeight(Properties.Resources.Loading.Width, Properties.Resources.Loading.Height));
            ImageBrush LoadingImageBrush = new ImageBrush();
            LoadingImageBrush.ImageSource = LoadingImage;

            InitializeComponent();

            this.Background = LoadingImageBrush;
        }
    }
}
