using System;
using System.Collections.Generic;

namespace MonoGameLibrary.Graphics;

public class Animation
{
    /// <summary>Danh sách các TextureRegion tạo nên các khung hình (frames).</summary>
    public List<TextureRegion> Frames { get; set; }

    /// <summary>Khoảng thời gian trì hoãn giữa mỗi khung hình.</summary>
    public TimeSpan Delay { get; set; }

    /// <summary>Số lượng khung hình.</summary>
    public int FrameCount => Frames?.Count ?? 0;

    /// <summary>Bắt buộc kết thúc animation</summary>
    public bool ForgeFinish { get; set; }

    /// <summary>Tạo một animation mới.</summary>
    public Animation()
    {
        Frames = new List<TextureRegion>();
        Delay = TimeSpan.FromMilliseconds(100);
    }

    /// <summary>Tạo một animation mới với các khung hình và độ trễ đã được chỉ định.</summary>
    /// <param name="frames">Tập hợp được sắp xếp (theo thứ tự) của các khung hình cho animation này.</param>
    /// <param name="delay">Khoảng thời gian trì hoãn giữa mỗi khung hình của animation này.</param>
    public Animation(List<TextureRegion> frames, TimeSpan delay, Boolean forgeFinish)
    {
        Frames = frames;
        Delay = delay;
        ForgeFinish = forgeFinish;
    }

    /// <summary>Lấy TextureRegion cho một khung hình cụ thể.</summary>
    public TextureRegion GetFrame(int frameIndex)
    {
        if (Frames == null || Frames.Count == 0) return null;

        // Đảm bảo chỉ số hợp lệ trước khi truy cập
        if (frameIndex >= 0 && frameIndex < Frames.Count)
        {
            return Frames[frameIndex];
        }
        // Trả về khung hình đầu tiên nếu chỉ số không hợp lệ
        return Frames[0];
    }

}
