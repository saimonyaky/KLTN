using Gum.DataTypes;
using Gum.Managers;
using Microsoft.Xna.Framework;
using MonoGameGum;
using MonoGameGum.GueDeriving;
using MonoGameLibrary.Graphics;

namespace RPGGame.UI
{
    internal class SpeechBubble : ContainerRuntime
    {
        private NineSliceRuntime _background;
        private SpriteRuntime _tail;
        private TextRuntime _textLabel;

        /// Creates a new TitlePanel instance using graphics from the specified texture atlas.
        /// <param name="atlas">The texture atlas containing title graphics.</param>
        public SpeechBubble(TextureAtlas atlas)
        {
            TextureRegion bodyRegion = atlas.GetRegion("speech-bubble");

            _background = new NineSliceRuntime();
            _background.Texture = atlas.Texture;
            _background.TextureAddress = TextureAddress.Custom;
            _background.TextureLeft = bodyRegion.SourceRectangle.Left;
            _background.TextureTop = bodyRegion.SourceRectangle.Top;
            _background.TextureWidth = bodyRegion.Width;
            _background.TextureHeight = bodyRegion.Height;
            _background.WidthUnits = DimensionUnitType.Absolute;
            _background.HeightUnits = DimensionUnitType.Absolute;
            AddChild(_background);

            _textLabel = new TextRuntime();
            _textLabel.UseCustomFont = true;
            _textLabel.CustomFontFile = @"fonts/04b_30.fnt"; // Đường dẫn font của bạn
            _textLabel.X = 10;
            _textLabel.Y = 10;
            _textLabel.Color = Color.Black;
            AddChild(_textLabel);

            // Mặc định ẩn
            this.Visible = false;

        }

        /// <summary>
        /// Hiển thị hội thoại tại vị trí cụ thể
        /// </summary>
        public void Show(float x, float y, string content)
        {
            this.Visible = true;

            // 1. Reset text
            _textLabel.Text = content;

            // 2. Xử lý xuống dòng (Word Wrap) thủ công
            // Lấy đối tượng BitmapFont thực sự để đo đạc
            var font = _textLabel.BitmapFont;

            if (font != null)
            {
                // Đo kích thước chuỗi text trên 1 dòng
                var size = font.MeasureString(content);

                //// Nếu chữ quá dài -> Cần ép chiều rộng để xuống dòng
                //if (size.X > _maxWidth)
                //{
                //    _textLabel.Width = _maxWidth;
                //    _textLabel.WidthUnits = DimensionUnitType.Absolute;

                //    // Ép TextRuntime tự tính lại chiều cao khi bị giới hạn chiều rộng
                //    // (Gum TextRuntime thường tự xử lý việc này nếu set Width)
                //}
                //else
                //{
                //    // Nếu chữ ngắn, chiều rộng = chiều rộng chữ
                //    _textLabel.Width = size.X;
                //    _textLabel.WidthUnits = DimensionUnitType.Absolute;
                //}

                // Cập nhật lại layout text để lấy chiều cao chính xác sau khi wrap
                // (Lưu ý: Đôi khi TextRuntime.Height chưa update ngay, ta có thể tính thủ công nếu cần)
                // Nhưng thường gán Width xong thì Height của TextRuntime sẽ tự nhảy.
            }

            // 3. ÉP KÍCH THƯỚC BACKGROUND
            // Công thức: Kích thước Text + (Padding * 2 phía)
            // Lưu ý: Cộng thêm một chút buffer (ví dụ +5f) nếu font render bị lẹm
            float safeBuffer = 5f;

            // Để lấy Height chính xác của text đa dòng, ta có thể dùng font.MeasureString với text đã wrap
            // Hoặc đơn giản hơn: dựa vào TextRuntime (hy vọng nó đã update)

            // Cách hack chắc ăn nhất để text không lòi ra ngoài:
            _background.Width = _textLabel.Width + (115 * 2) + safeBuffer;

            // Đo lại chiều cao thực tế của TextRuntime (sau khi đã set Width và Text)
            // Nếu TextRuntime chưa update, ta có thể dùng font.MeasureString(wrappedText).Y
            // Ở đây ta giả định TextRuntime tự update (thường đúng với Gum MonoGame)
            _background.Height = _textLabel.Height + (120 * 2) + safeBuffer;


            //// 4. CẬP NHẬT LẠI VỊ TRÍ ĐUÔI (Vì Background vừa đổi kích thước)
            //_tail.X = (_background.Width / 2) - (_tail.Width / 2);
            //_tail.Y = _background.Height - 2f; // Trừ 2f để chồng lấp nhẹ che khe hở

            // 5. ĐẶT VỊ TRÍ TỔNG THỂ
            // (x, y) là điểm nhọn của đuôi
            this.X = x - (_background.Width / 2);
            this.Y = y - _background.Height;
        }

        public void Hide()
        {
            this.Visible = false;
        }
    }
}
