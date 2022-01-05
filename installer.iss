; replace constant autoappdata by autodocs

#define MyAppName "GRBL-Plotter"
#define MyAppExeName "GRBL-Plotter.exe"
#define MyAppPublisher "GRBL-Plotter"
#define MyAppURL "https://grbl-plotter.de"
#define MyReleasePath "GRBL-Plotter\bin\Release\"
#define MyWorkPath "GRBL-Plotter\"
#define MyAppVersion GetVersionNumbersString("GRBL-Plotter\bin\Release\GRBL-Plotter.exe")
#define MySetupVersion "1.0.2.0"
#define MyAppDataPath "\GRBL-Plotter"

[Setup]
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppId=8079d7d8-2b91-4a22-a13e-7e5ac5a9e5fc
AppPublisher=svenhb
AppPublisherURL=https://grbl-plotter.de/
VersionInfoVersion= {#MySetupVersion}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputDir=Output
OutputBaseFilename=GRBL-Plotter_Setup
AppCopyright=Copyright © 2015-2022 SH
WizardImageFile=image_modell_328.bmp
WizardSmallImageFile=image_modell_55.bmp
WizardStyle=modern
WizardSizePercent=100
Compression=zip
SolidCompression=no
InternalCompressLevel=ultra64
UsePreviousPrivileges=no
PrivilegesRequiredOverridesAllowed=dialog
InfoBeforeFile=readme.txt

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"
; from https://github.com/kira-96/Inno-Setup-Chinese-Simplified-Translation
Name: "cn"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"
Name: "ru"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ja"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "pt"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "sp"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "fr"; MessagesFile: "compiler:Languages\French.isl"
Name: "cs"; MessagesFile: "compiler:Languages\Czech.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#MyReleasePath}GRBL-Plotter.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleasePath}GRBL-Plotter.pdb"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleasePath}NLog.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleasePath}*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyReleasePath}ar\*"; DestDir: "{app}\ar"; Flags: ignoreversion
Source: "{#MyReleasePath}cs\*"; DestDir: "{app}\cs"; Flags: ignoreversion
Source: "{#MyReleasePath}de-DE\*"; DestDir: "{app}\de-DE"; Flags: ignoreversion
Source: "{#MyReleasePath}es\*"; DestDir: "{app}\es"; Flags: ignoreversion
Source: "{#MyReleasePath}fr\*"; DestDir: "{app}\fr"; Flags: ignoreversion
Source: "{#MyReleasePath}ja\*"; DestDir: "{app}\ja"; Flags: ignoreversion
Source: "{#MyReleasePath}pt\*"; DestDir: "{app}\pt"; Flags: ignoreversion
Source: "{#MyReleasePath}ru\*"; DestDir: "{app}\ru"; Flags: ignoreversion
Source: "{#MyReleasePath}zh-CN\*"; DestDir: "{app}\zh-CN"; Flags: ignoreversion

Source: "{#MyWorkPath}Recent.txt"; DestDir: "{autodocs}{#MyAppDataPath}"; Flags: comparetimestamp promptifolder; Permissions: users-modify
Source: "{#MyReleasePath}data\*"; DestDir: "{autodocs}{#MyAppDataPath}\data"; Flags: recursesubdirs comparetimestamp promptifolder; Permissions: users-modify

Source: "Firmware\*"; DestDir: "{autodocs}{#MyAppDataPath}\Firmware"; Flags: recursesubdirs; Permissions: users-modify

[Registry]
Root: HKA; Subkey: "Software\{#MyAppName}"; ValueType: string; ValueName: "DataPath"; ValueData: "{autodocs}{#MyAppDataPath}"; Flags: uninsdeletekey

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#MyAppName}}"; Flags: nowait postinstall skipifsilent
