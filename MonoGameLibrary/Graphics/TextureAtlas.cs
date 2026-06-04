using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameLibrary.Graphics;

public class TextureAtlas
{
    private Dictionary<string, TextureRegion> _regions;

    // Stores animations added to this atlas.
    private Dictionary<string, Animation> _animations;

    /// Gets or Sets the source texture represented by this texture atlas.
    public Texture2D Texture { get; set; }

    /// Creates a new texture atlas.
    public TextureAtlas()
    {
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// Creates a new texture atlas instance using the given texture.
    /// <param name="texture">The source texture represented by the texture atlas.</param>
    public TextureAtlas(Texture2D texture)
    {
        Texture = texture;
        _regions = new Dictionary<string, TextureRegion>();
        _animations = new Dictionary<string, Animation>();
    }

    /// Creates a new region and adds it to this texture atlas.
    /// <param name="name">The name to give the texture region.</param>
    /// <param name="x">The top-left x-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
    /// <param name="y">The top-left y-coordinate position of the region boundary relative to the top-left corner of the source texture boundary.</param>
    /// <param name="width">The width, in pixels, of the region.</param>
    /// <param name="height">The height, in pixels, of the region.</param>
    public void AddRegion(string name, int x, int y, int width, int height)
    {
        TextureRegion region = new TextureRegion(Texture, x, y, width, height);
        _regions.Add(name, region);
    }

    /// Gets the region from this texture atlas with the specified name.
    /// <param name="name">The name of the region to retrieve.</param>
    /// <returns>The TextureRegion with the specified name.</returns>
    public TextureRegion GetRegion(string name)
    {
        return _regions[name];
    }

    /// Removes the region from this texture atlas with the specified name.
    /// <param name="name">The name of the region to remove.</param>
    /// <returns></returns>
    public bool RemoveRegion(string name)
    {
        return _regions.Remove(name);
    }

    /// Removes all regions from this texture atlas.
    public void Clear()
    {
        _regions.Clear();
    }

    /// Creates a new texture atlas based on a texture atlas xml configuration file.
    /// <param name="content">The content manager used to load the texture for the atlas.</param>
    /// <param name="fileName">The path to the xml file, relative to the content root directory.</param>
    /// <returns>The texture atlas created by this method.</returns>
    public static TextureAtlas FromFile(ContentManager content, string fileName)
    {
        TextureAtlas atlas = new TextureAtlas();

        string filePath = Path.Combine(content.RootDirectory, fileName);

        using (Stream stream = TitleContainer.OpenStream(filePath))
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                // The <Texture> element contains the content path for the Texture2D to load.
                // So we will retrieve that value then use the content manager to load the texture.
                string texturePath = root.Element("Texture").Value;
                atlas.Texture = content.Load<Texture2D>(texturePath);

                // The <Regions> element contains individual <Region> elements, each one describing
                // a different texture region within the atlas.
                // So we retrieve all of the <Region> elements then loop through each one
                // and generate a new TextureRegion instance from it and add it to this atlas.
                var regions = root.Element("Regions")?.Elements("Region");

                if (regions != null)
                {
                    foreach (var region in regions)
                    {
                        string name = region.Attribute("name")?.Value;
                        int x = int.Parse(region.Attribute("x")?.Value ?? "0");
                        int y = int.Parse(region.Attribute("y")?.Value ?? "0");
                        int width = int.Parse(region.Attribute("width")?.Value ?? "0");
                        int height = int.Parse(region.Attribute("height")?.Value ?? "0");

                        if (!string.IsNullOrEmpty(name))
                        {
                            atlas.AddRegion(name, x, y, width, height);
                        }
                    }
                }

                // The <Animations> element contains individual <Animation> elements, each one describing
                // a different animation within the atlas.
                // So we retrieve all of the <Animation> elements then loop through each one
                // and generate a new Animation instance from it and add it to this atlas.
                var animationElements = root.Element("Animations").Elements("Animation");

                if (animationElements != null)
                {
                    foreach (var animationElement in animationElements)
                    {
                        string name = animationElement.Attribute("name")?.Value;
                        string regionName = animationElement.Attribute("region")?.Value;
                        float delayInMilliseconds = float.Parse(animationElement.Attribute("delay")?.Value ?? "0");
                        int frameWidth = int.Parse(animationElement.Attribute("frameWidth")?.Value ?? "0");
                        int frameHeight = int.Parse(animationElement.Attribute("frameHeight")?.Value ?? "0");
                        int frameCount = int.Parse(animationElement.Attribute("frameCount")?.Value ?? "1");
                        Boolean forgeFinish = bool.Parse(animationElement.Attribute("forgeFinish")?.Value ?? "false");

                        // Lấy Region lớn từ Atlas
                        TextureRegion region = atlas.GetRegion(regionName);

                        // Cắt Region lớn thành các Frame nhỏ (sử dụng logic Slice nội bộ)
                        List<TextureRegion> frames = SliceRegion(
                            region,
                            frameWidth,
                            frameHeight,
                            frameCount
                        );

                        // Tạo Animation và lưu trữ
                        TimeSpan delay = TimeSpan.FromMilliseconds(delayInMilliseconds);
                        Animation animation = new Animation(frames, delay, forgeFinish);
                        atlas._animations.Add(name, animation);
                    }
                }

                return atlas;
            }
        }
    }

    /// <summary>
    /// Creates a new sprite using the region from this texture atlas with the specified name.
    /// </summary>
    /// <param name="regionName">The name of the region to create the sprite with.</param>
    /// <returns>A new Sprite using the texture region with the specified name.</returns>
    public Sprite CreateSprite(string regionName)
    {
        TextureRegion region = GetRegion(regionName);
        return new Sprite(region);
    }

    /// <summary>Thực hiện cắt một TextureRegion lớn thành danh sách các khung hình (frame) nhỏ đều nhau.</summary>
    /// <param name="fullRegion">Tên của Region lớn cần cắt.</param>
    /// <param name="frameWidth">Chiều rộng của mỗi khung hình.</param>
    /// <param name="frameHeight">Chiều cao của mỗi khung hình.</param>
    /// <param name="frameCount">Số lượng khung hình cần lấy.</param>
    /// <returns>Danh sách các TextureRegion đại diện cho các Frame.</returns>
    private static List<TextureRegion> SliceRegion(TextureRegion fullRegion, int frameWidth, int frameHeight, int frameCount)
    {
        var frames = new List<TextureRegion>();
        int columns = fullRegion.Width / frameWidth;

        int totalFramesInSheet = columns * (fullRegion.Height / frameHeight);
        int limit = Math.Min(frameCount, totalFramesInSheet);

        for (int i = 0; i < limit; i++)
        {
            int x = i % columns * frameWidth;
            int y = i / columns * frameHeight;

            frames.Add(new TextureRegion(
                fullRegion.Texture,
                fullRegion.SourceRectangle.X + x,
                fullRegion.SourceRectangle.Y + y,
                frameWidth,
                frameHeight
            ));
        }
        return frames;
    }

    /// <summary>
    /// Adds the given animation to this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to add.</param>
    /// <param name="animation">The animation to add.</param>
    public void AddAnimation(string animationName, Animation animation)
    {
        _animations.Add(animationName, animation);
    }

    /// <summary>
    /// Gets the animation from this texture atlas with the specified name.
    /// </summary>
    /// <param name="animationName">The name of the animation to retrieve.</param>
    /// <returns>The animation with the specified name.</returns>
    public Animation GetAnimation(string animationName)
    {
        return _animations[animationName];
    }

    /// <summary>
    /// Removes the animation with the specified name from this texture atlas.
    /// </summary>
    /// <param name="animationName">The name of the animation to remove.</param>
    /// <returns>true if the animation is removed successfully; otherwise, false.</returns>
    public bool RemoveAnimation(string animationName)
    {
        return _animations.Remove(animationName);
    }

}
