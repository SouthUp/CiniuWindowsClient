using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace CheckWordModel
{
    /// <summary>
    /// Wraps recognized word and provides word overlay position and size based on current scale.
    /// </summary>
    public class WordOverlay : INotifyPropertyChanged
    {
        private Rect BoundingRect;

        /// <summary>
        /// Left and Right properties of Thickess define word box position.
        /// </summary>
        public Thickness WordPosition { get; private set; }

        /// <summary>
        /// Scaled word box width.
        /// </summary>
        public double WordWidth { get; private set; }

        /// <summary>
        /// Scaled word box height.
        /// </summary>
        public double WordHeight { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public WordOverlay(Rect boundingRect)
        {
            BoundingRect = boundingRect;

            UpdateProps(BoundingRect);
        }

        public void Transform(ScaleTransform scale)
        {
            // Scale word box bounding rect and update properties.
            UpdateProps(scale.TransformBounds(BoundingRect));
        }

        public Binding CreateWordPositionBinding()
        {
            Binding positionBinding = new Binding()
            {
                Path = new PropertyPath("WordPosition"),
                Source = this
            };

            return positionBinding;
        }

        public Binding CreateWordWidthBinding()
        {
            Binding widthBinding = new Binding()
            {
                Path = new PropertyPath("WordWidth"),
                Source = this
            };

            return widthBinding;
        }

        public Binding CreateWordHeightBinding()
        {
            Binding heightBinding = new Binding()
            {
                Path = new PropertyPath("WordHeight"),
                Source = this
            };

            return heightBinding;
        }

        private void UpdateProps(Rect wordBoundingBox)
        {
            WordPosition = new Thickness(wordBoundingBox.Left, wordBoundingBox.Top, 0, 0);
            WordWidth = wordBoundingBox.Width;
            WordHeight = wordBoundingBox.Height;

            OnPropertyChanged("WordPosition");
            OnPropertyChanged("WordWidth");
            OnPropertyChanged("WordHeight");
        }

        protected void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
    }
}
