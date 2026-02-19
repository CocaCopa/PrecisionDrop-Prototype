namespace PrecisionDrop.Platforms.Contracts {
    public interface IPlatformBuilder {
        int PlatformSegments { get; }
        void Create(PlatformConfig config);
    }
}
