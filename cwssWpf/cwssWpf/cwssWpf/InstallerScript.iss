[Setup]
AppName=CWSS System
AppVersion=1.0
DefaultDirName={pf}\CWSS System
DefaultGroupName=CWSS System
UninstallDisplayIcon={app}\cwssWpf.exe
Compression=lzma2
SolidCompression=yes
OutputDir=C:\Users\Derek\Source\Repos\CWSSv2\cwssWpf\cwssWpf\cwssWpf\bin\Release\Setup
OutputBaseFilename=CWSS Setup
PrivilegesRequired=admin

[Components]
Name: client; Description: Install for a remote client machine.; Flags: Dontinheritcheck

[Files]
Source: "cwssWpf.exe"; DestDir: "{app}"
Source: "ImageProcessor.dll"; DestDir: "{app}"
Source: "itextsharp.dll"; DestDir: "{app}"
Source: "Newtonsoft.Json.dll"; DestDir: "{app}"
Source: "RijndaelEncryptDecrypt.dll"; DestDir: "{app}"
Source: "WebEye.Controls.Wpf.WebCameraControl.dll"; DestDir: "{app}"
Source: "UserManual.txt"; DestDir: "{app}"; Flags: isreadme

Source: "AppData\ClientSetup.cfg"; DestDir: "{app}\AppData"; Components: client

Source: "Sounds\CheckIn.wav"; DestDir: "{app}\Sounds"
Source: "Sounds\CheckOut.wav"; DestDir: "{app}\Sounds"
Source: "Sounds\Fail.wav"; DestDir: "{app}\Sounds"
Source: "Sounds\LogIn.wav"; DestDir: "{app}\Sounds"
Source: "Sounds\LogOff.wav"; DestDir: "{app}\Sounds"
Source: "Sounds\Success.wav"; DestDir: "{app}\Sounds"

Source: "Images\Connect.png"; DestDir: "{app}\Images"
Source: "Images\Disconnect.png"; DestDir: "{app}\Images"
Source: "Images\Alert.png"; DestDir: "{app}\Images"
Source: "Images\Success.png"; DestDir: "{app}\Images"
Source: "Images\Waiver.png"; DestDir: "{app}\Images"

Source: "AppData\CW Acknowledgement of Risk and Sign-In Sheet.pdf"; DestDir: "{app}\AppData"

[Icons]
Name: "{group}\CWSS System"; Filename: "{app}\cwssWpf.exe"

[Run]
Filename: "{app}\cwssWpf.exe"; Description: "{cm:LaunchProgram,MyApp}"; Flags: runascurrentuser nowait postinstall skipifsilent
