using System;

namespace PrecisionDrop.LevelGeneration.Runtime {
    [Serializable]
    internal struct GenerationSettings {
        public FirstBatchConfig firstBatchConfig;
        public FullDangerSectionSettings fullDangerSectionSettings;
        public GeneralSettings generalSettings;
        public GapConfig[] gapConfigs;
        public DangerConfig dangerConfig;

        public GenerationSettings(FirstBatchConfig firstBatchConfig, FullDangerSectionSettings fullDangerSectionSettings, GeneralSettings generalSettings, GapConfig[] gapConfigs, DangerConfig dangerConfig) {
            this.firstBatchConfig = firstBatchConfig;
            this.fullDangerSectionSettings = fullDangerSectionSettings;
            this.generalSettings = generalSettings;
            this.gapConfigs = gapConfigs;
            this.dangerConfig = dangerConfig;
        }
    }
}