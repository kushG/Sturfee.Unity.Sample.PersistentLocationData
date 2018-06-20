# Sturfee StreetAR SDK
## Release Notes

### Version 0.9.1

**Bug Fixes**

- Debug building geometry `ON` by default (Tier 3); Will now be `OFF` by default
- `SturfeeXRLight` prefab's shadows are turned off
- Issue with light estimation from `DebugLightingProvider` (orientation and intensity)

**API Changes** 

_BREAKING Changes_
- Event `OnAlignmentComplete` changed to `OnLocalizationComplete`
- Event `OnAlignmentLoading` changed to `OnLocalizationLoading`
- Event `OnHitscanComplete` changed to `OnDetectSurfacePointComplete`
- Event `OnHitScanLoading` changed to `OnDetectSurfacePointLoading`
- Event `OnHitscanFailed` changed to `OnDetectSurfacePointFailed`
- Event `OnAlignmentCheckComplete` changed to `OnCoverageCheckComplete`

_Deprication (will be removed in subsequent release)_
- Added `XRSessionManager.PerformLocalization()` to replace `XRSesssionManager.PerformAlignment()`
- Added `XRSessionManager.CheckCoverage()` to replace `XRSesssionManager.CheckAlignment()`
- Added `XRSessionManager.DetectSurfaceAtPoint()` to replace `XRSesssionManager.PerformHitscan()`

**General Changes**
- Moved `UnityARkitPlugin` from `Aseets/Sturfee/Plugins/External/` to `Assets/` folder
- Added Template code for new projects in `Assets/Sturfee/Templates` folder



