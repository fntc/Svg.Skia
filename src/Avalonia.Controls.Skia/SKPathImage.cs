﻿using System;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace Avalonia.Controls.Skia;

/// <summary>
/// An <see cref="IImage"/> that uses a <see cref="SKPath"/> for content.
/// </summary>
public class SKPathImage : AvaloniaObject, IImage, IAffectsRender
{
    /// <summary>
    /// Defines the <see cref="Source"/> property.
    /// </summary>
    public static readonly StyledProperty<SKPath?> SourceProperty =
        AvaloniaProperty.Register<SKPathImage, SKPath?>(nameof(Source));

    /// <summary>
    /// Defines the <see cref="Paint"/> property.
    /// </summary>
    public static readonly StyledProperty<SKPaint?> PaintProperty =
        AvaloniaProperty.Register<SKPathImage, SKPaint?>(nameof(Paint));

    public event EventHandler? Invalidated;

    /// <summary>
    /// Gets or sets the <see cref="SKPath"/> content.
    /// </summary>
    [Content]
    public SKPath? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="SKPaint"/> paint.
    /// </summary>
    public SKPaint? Paint
    {
        get => GetValue(PaintProperty);
        set => SetValue(PaintProperty, value);
    }

    /// <inheritdoc/>
    public Size Size => Source is { } ? new Size(Source.Bounds.Width, Source.Bounds.Height) : default;

    /// <inheritdoc/>
    void IImage.Draw(DrawingContext context, Rect sourceRect, Rect destRect, BitmapInterpolationMode bitmapInterpolationMode)
    {
        var source = Source;
        if (source is null)
        {
            return;
        }
        var bounds = source.Bounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }
        var paint = Paint;
        if (paint is null)
        {
            return;
        }
        var scaleMatrix = Matrix.CreateScale(destRect.Width / sourceRect.Width, destRect.Height / sourceRect.Height);
        var translateMatrix = Matrix.CreateTranslation(-sourceRect.X + destRect.X - bounds.Top, -sourceRect.Y + destRect.Y - bounds.Left);
        using (context.PushClip(destRect))
        using (context.PushTransform(translateMatrix * scaleMatrix))
        {
            context.Custom(new SKPathDrawOperation(new Rect(0, 0, bounds.Width, bounds.Height), source, paint));
        }
    }

    /// <inheritdoc/>
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SourceProperty)
        {
            RaiseInvalidated(EventArgs.Empty);
        }
    }

    /// <summary>
    /// Raises the <see cref="Invalidated"/> event.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected void RaiseInvalidated(EventArgs e) => Invalidated?.Invoke(this, e);
}
