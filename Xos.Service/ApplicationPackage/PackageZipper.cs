using System.IO.Compression;
using System.Text;

namespace Xos.Service.ApplicationPackage;

/// <summary>
/// GZip 压缩工具类，提供字符串压缩和解压功能
/// </summary>
static class PackageZipper
{
    // 默认缓冲区大小 (1MB)
    private const int DefaultBufferSize = 1024 * 1024;

    /// <summary>
    /// 压缩字符串并保存到文件
    /// </summary>
    /// <param name="text">要压缩的文本</param>
    /// <param name="filePath">保存路径</param>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="progressCallback">进度回调 (0-1)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <param name="bufferSize">缓冲区大小 (可选)</param>
    /// <returns>压缩结果信息</returns>
    public static CompressionResult CompressStringToFile(
        string text,
        string filePath,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        Action<double>? progressCallback = null,
        CancellationToken cancellationToken = default,
        int bufferSize = DefaultBufferSize)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("输入文本不能为空", nameof(text));

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("文件路径不能为空", nameof(filePath));

        if (bufferSize <= 0)
            throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        long originalSize = Encoding.UTF8.GetByteCount(text);
        long compressedSize = 0;
        var startTime = DateTime.UtcNow;

        try
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            long totalBytes = textBytes.Length;
            long processedBytes = 0;

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var gzipStream = new GZipStream(fileStream, compressionLevel))
            {
                int offset = 0;

                while (offset < textBytes.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int bytesToWrite = Math.Min(bufferSize, textBytes.Length - offset);
                    gzipStream.Write(textBytes, offset, bytesToWrite);

                    offset += bytesToWrite;
                    processedBytes += bytesToWrite;

                    // 计算并报告进度
                    double progress = (double)processedBytes / totalBytes;
                    progressCallback?.Invoke(progress);
                }
            }

            // 获取最终压缩大小
            compressedSize = new FileInfo(filePath).Length;
            var duration = DateTime.UtcNow - startTime;

            return new CompressionResult(
                success: true,
                originalSize: originalSize,
                compressedSize: compressedSize,
                duration: duration);
        }
        catch (OperationCanceledException)
        {
            // 清理部分压缩的文件
            if (File.Exists(filePath)) File.Delete(filePath);
            return CompressionResult.CancelledResult;
        }
        catch (Exception ex)
        {
            return new CompressionResult(
                success: false,
                error: ex.Message,
                originalSize: originalSize,
                compressedSize: compressedSize);
        }
    }

    /// <summary>
    /// 从文件解压字符串
    /// </summary>
    /// <param name="filePath">GZip 文件路径</param>
    /// <param name="progressCallback">进度回调 (0-1)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <param name="bufferSize">缓冲区大小 (可选)</param>
    /// <returns>解压后的字符串</returns>
    public static string DecompressStringFromFile(
        string filePath,
        Action<double>? progressCallback = null,
        CancellationToken cancellationToken = default,
        int bufferSize = DefaultBufferSize)
    {
        if (!File.Exists(filePath))
            return string.Empty;

        if (bufferSize <= 0)
            throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        var fileInfo = new FileInfo(filePath);
        long totalBytes = fileInfo.Length;
        long processedBytes = 0;

        using (var fileStream = new FileStream(filePath, FileMode.Open))
        using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
        using (var decompressedStream = new MemoryStream())
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                decompressedStream.Write(buffer, 0, bytesRead);
                processedBytes += bytesRead;

                // 计算并报告进度
                double progress = (double)processedBytes / totalBytes;
                progressCallback?.Invoke(progress);
            }

            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }

    /// <summary>
    /// 压缩字符串到字节数组
    /// </summary>
    /// <param name="text">要压缩的文本</param>
    /// <param name="compressionLevel">压缩级别</param>
    /// <param name="progressCallback">进度回调 (0-1)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <param name="bufferSize">缓冲区大小 (可选)</param>
    /// <returns>压缩后的字节数组</returns>
    public static byte[] CompressStringToBytes(
        string text,
        CompressionLevel compressionLevel = CompressionLevel.Optimal,
        Action<double>? progressCallback = null,
        CancellationToken cancellationToken = default,
        int bufferSize = DefaultBufferSize)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("输入文本不能为空", nameof(text));

        if (bufferSize <= 0)
            throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        long totalBytes = textBytes.Length;
        long processedBytes = 0;

        using (var memoryStream = new MemoryStream())
        {
            using (var gzipStream = new GZipStream(memoryStream, compressionLevel, true))
            {
                int offset = 0;

                while (offset < textBytes.Length)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    int bytesToWrite = Math.Min(bufferSize, textBytes.Length - offset);
                    gzipStream.Write(textBytes, offset, bytesToWrite);

                    offset += bytesToWrite;
                    processedBytes += bytesToWrite;

                    // 计算并报告进度
                    double progress = (double)processedBytes / totalBytes;
                    progressCallback?.Invoke(progress);
                }
            }

            return memoryStream.ToArray();
        }
    }

    /// <summary>
    /// 从字节数组解压字符串
    /// </summary>
    /// <param name="compressedData">压缩的字节数组</param>
    /// <param name="progressCallback">进度回调 (0-1)</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <param name="bufferSize">缓冲区大小 (可选)</param>
    /// <returns>解压后的字符串</returns>
    public static string DecompressStringFromBytes(
        byte[] compressedData,
        Action<double>? progressCallback = null,
        CancellationToken cancellationToken = default,
        int bufferSize = DefaultBufferSize)
    {
        if (compressedData == null || compressedData.Length == 0)
            throw new ArgumentException("压缩数据不能为空", nameof(compressedData));

        if (bufferSize <= 0)
            throw new ArgumentException("缓冲区大小必须大于0", nameof(bufferSize));

        long totalBytes = compressedData.Length;
        long processedBytes = 0;

        using (var memoryStream = new MemoryStream(compressedData))
        using (var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
        using (var decompressedStream = new MemoryStream())
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = gzipStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                decompressedStream.Write(buffer, 0, bytesRead);
                processedBytes += bytesRead;

                // 计算并报告进度
                double progress = (double)processedBytes / totalBytes;
                progressCallback?.Invoke(progress);
            }

            return Encoding.UTF8.GetString(decompressedStream.ToArray());
        }
    }

    /// <summary>
    /// 估算压缩后的文件大小
    /// </summary>
    /// <param name="text">原始文本</param>
    /// <param name="compressionLevel">压缩级别</param>
    /// <returns>预估的压缩大小</returns>
    public static long EstimateCompressedSize(string text, CompressionLevel compressionLevel = CompressionLevel.Optimal)
    {
        if (string.IsNullOrWhiteSpace(text)) return 0;

        // 简单估算：压缩率通常在 60-90% 之间
        double compressionRatio = compressionLevel switch
        {
            CompressionLevel.Fastest => 0.7, // 30% 压缩率
            CompressionLevel.Optimal => 0.5, // 50% 压缩率
            CompressionLevel.SmallestSize => 0.4, // 60% 压缩率
            _ => 0.5
        };

        long originalSize = Encoding.UTF8.GetByteCount(text);
        return (long)(originalSize * compressionRatio);
    }

    /// <summary>
    /// 获取压缩文件信息
    /// </summary>
    /// <param name="filePath">压缩文件路径</param>
    /// <returns>文件信息对象</returns>
    public static CompressionFileInfo GetFileInfo(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("文件不存在", filePath);

        var fileInfo = new FileInfo(filePath);

        // 尝试获取原始大小
        long originalSize = 0;
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                // GZip 格式在最后4个字节存储原始大小
                stream.Seek(-4, SeekOrigin.End);
                var sizeBuffer = new byte[4];
                stream.Read(sizeBuffer, 0, 4);
                originalSize = BitConverter.ToInt32(sizeBuffer, 0);
            }
        }
        catch
        {
            // 如果无法获取，使用默认值
            originalSize = 0;
        }

        return new CompressionFileInfo(
            filePath: filePath,
            compressedSize: fileInfo.Length,
            originalSize: originalSize,
            created: fileInfo.CreationTimeUtc,
            modified: fileInfo.LastWriteTimeUtc);
    }
}

/// <summary>
/// 压缩结果信息
/// </summary>
public class CompressionResult
{
    public static CompressionResult CancelledResult =>
        new CompressionResult(success: false, error: "操作已取消");

    public bool Success { get; }
    public string? Error { get; }
    public long OriginalSize { get; }
    public long CompressedSize { get; }
    public TimeSpan Duration { get; }

    public double CompressionRatio =>
        OriginalSize > 0 ? (double)CompressedSize / OriginalSize * 100 : 0;

    public CompressionResult(
        bool success,
        string? error = null,
        long originalSize = 0,
        long compressedSize = 0,
        TimeSpan duration = default)
    {
        Success = success;
        Error = error;
        OriginalSize = originalSize;
        CompressedSize = compressedSize;
        Duration = duration;
    }

    public override string ToString()
    {
        if (!Success) return $"压缩失败: {Error}";

        return $"压缩率: {CompressionRatio:F1}% | " +
               $"原始大小: {FormatBytes(OriginalSize)} | " +
               $"压缩后: {FormatBytes(CompressedSize)} | " +
               $"耗时: {Duration.TotalSeconds:F2}秒";
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}

/// <summary>
/// 压缩文件信息
/// </summary>
public class CompressionFileInfo
{
    public string FilePath { get; }
    public long CompressedSize { get; }
    public long OriginalSize { get; }
    public DateTime Created { get; }
    public DateTime Modified { get; }

    public double CompressionRatio =>
        OriginalSize > 0 ? (double)CompressedSize / OriginalSize * 100 : 0;

    public CompressionFileInfo(
        string filePath,
        long compressedSize,
        long originalSize,
        DateTime created,
        DateTime modified)
    {
        FilePath = filePath;
        CompressedSize = compressedSize;
        OriginalSize = originalSize;
        Created = created;
        Modified = modified;
    }

    public override string ToString()
    {
        return $"文件: {Path.GetFileName(FilePath)} | " +
               $"压缩率: {CompressionRatio:F1}% | " +
               $"原始大小: {FormatBytes(OriginalSize)} | " +
               $"压缩后: {FormatBytes(CompressedSize)}";
    }

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}