# Release policy

Every public version of Quick Print Studio must receive a new semantic version and its own GitHub Release.

Rules:
- Never delete an older public release as part of publishing a new version.
- Never overwrite an older versioned installer.
- Installer names must include the version: QuickPrintStudio-X.Y.Z-Setup.exe.
- Tags use vX.Y.Z.
- Release notes must state validation level.
- A CorelDRAW installer may be labelled validated only after compilation and smoke testing on Windows with CorelDRAW 2025/VGCore.
- CI-only source packages must not be described as tested CorelDRAW installers.
