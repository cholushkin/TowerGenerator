# Changelog

## [1.0.0] - UPM conversion
- Added `package.json` (`com.qcat.towergenerator`), declaring `com.unity.mathematics` as a Unity Registry dependency.
- Added `LICENSE` (MIT).
- Documented required manual Git/third-party dependencies in README: `com.qcat.gamelib`, Alchemy, UniTask, VitalRouter, ZLogger, Microsoft.Extensions.Logging.
- Flagged a pre-existing Runtime→Editor asmdef reference issue in README (not fixed, not introduced by this conversion).
- No source, asmdef, namespace, or GUID changes. Existing `VersionHistory/` tooling folder left untouched.
