# GRBL-Plotter Linux

## Prerequirements

### Install steam (Read Additional Infos if you don't have a steam account):

- please use the steam version of your distros repos. The flatpak version might cause some issues and might not work.
  
  - Ubuntu/Debian:
    
    ```bash
    sudo apt install steam
    ```
    
    Arch Linux:
    
    ```bash
    sudo pacman -S steam
    ```
    
    RHEL-based (Fedora):
    
    ```bash
    sudo dnf install steam
    ```
  
  **Additional infos:**

- WARNING: You need a steam Account to use the Software. If you don't want to use Steam you can use [GitHub - GloriousEggroll/wine-ge-custom](https://github.com/GloriousEggroll/wine-ge-custom). But it might be more complicated and I highly recommend to manage Windows Software with Steam even if you don't play video games. It makes life much easier. If you already tried it please open an Github Issue with the setup guide for wine-ge-custom.

- Why not wine instead of proton? proton has more features and fixes and grbl-plotter runs without any problems using proton, while it doesn't using wine. I didn't get it working on wine so far.

### Install protontricks and wine

- Install wine
  
  - Ubuntu/Debian:
    
    ```bash
    sudo apt install wine
    ```
    
    Arch Linux:
    
    ```bash
    sudo pacman -S wine
    ```
    
    RHEL-based (Fedora):
    
    ```bash
    sudo dnf install wine
    ```

- The installation of Protontricks does vary depending on your distribution. Check if there is a community package for it:
  
  - Arch: [AUR (en) - protontricks](https://aur.archlinux.org/packages/protontricks)
  
  - Debian: https://sources.debian.org/src/protontricks/
  
  - Fedora: [protontricks - Fedora Packages](https://packages.fedoraproject.org/pkgs/protontricks/protontricks/)

### Install Microsoft Fonts

- There might be a community package for that:
  
  - Arch: [AUR (en) - ttf-ms-win11-auto](https://aur.archlinux.org/packages/ttf-ms-win11-auto) (don't forget to install  `p7zip` and `httpdirfs` before the AUR package)
  
  - Debian (not tested): [Debian -- ttf-mscorefonts-installer](https://packages.debian.org/search?keywords=ttf-mscorefonts-installer)
  
  - Fedora (not tested):
    
    ```bash
    sudo dnf install curl cabextract xorg-x11-font-utils fontconfig
    sudo rpm -i https://downloads.sourceforge.net/project/mscorefonts2/rpms/msttcore-fonts-installer-2.6-1.noarch.rpm
    ```

- After some might not work (grbl-plotter won't start) there is the possibility to get the fonts from original Windows ISO. Thats How it works:
  
  - Download the Windows ISO:
    [Download Windows 11](https://www.microsoft.com/en-gb/software-download/windows11)
  
  - Ubuntu/Debian:
    
    ```bash
    sudo apt install p7zip-full
    ```
    
    Arch Linux:
    
    ```bash
    sudo pacman -S p7zip
    ```
    
    RHEL-based (Fedora):
    
    ```bash
    sudo dnf install p7zip-full
    ```
  
  - Extract "install.wim" from ISO
    
    ```bash
    7z e "{ISONAME}.iso" sources/install.wim
    ```
  
  - Extract Fonts folder
    
    ```bash
    7z e install.wim 1/Windows/{Fonts/"*".{ttf,ttc},System32/Licenses/neutral/"*"/"*"/license.rtf} -o./WindowsFonts
    ```
  
  - Move WindowsFonts folder
    
    ```bash
    sudo mv ./WindowsFonts /usr/share/fonts/
    ```
  
  - Regenerate font cache
    
    ```bash
    sudo fc-cache -fv
    ```

## Install GRBL-Plotter

1. Download the GRBL-Plotter_Setup.exe [Releases · svenhb/GRBL-Plotter · GitHub](https://github.com/svenhb/GRBL-Plotter/releases)

2. Open Steam and click on "Add a Game" at the bottom left

3. Select "Add non-Steam Game...", click on "Browse..." and select the GRBL-Plotter_Setup.exe. Then click on "Add selected programs"

4. Switch to the Library-Tab and scroll to GRBL-Plotter_Setup.exe in list on the left and select it. Then open the properties by clicking the cogwheel on the right.

5. Open "Compatibility" and activate Steam Play. Then close the properties window and start the installer by pressing "Play". Steam downloads all necessary dependencies and the installer opens.

6. Install GRBL Plotter for all Users and uncheck "Start GRBL-Plotter" at the end of the installation.

7. Open Protontricks, search "GRBL-Plotter_Setup.exe" and save the 10 digit ID behind in your notepad (in this guide: {GRBL-STEAM-ID}). We will need it later to identify the right folder which has the same name as the ID. Then use this terminal command to install gdi+ for GRBL-Plotter:
   
   ```bash
   protontrick {GRBL-STEAM-ID} gdiplus
   ```

8. We have to change the path of the Steam Program. Open the Properties of GRBL-Plotter_Setup.exe again and change the Paths. Replace {USERNAME} with your username. Don't forget the quotation marks.
   
   - TARGET:
     
     ```bash
     "/home/{USERNAME}/.steam/steamapps/compatdata/{GRBL-STEAM-ID}/pfx/drive_c/Program Files (x86)/GRBL-Plotter/GRBL-Plotter.exe""
     ```
   
   - START IN:
     
     ```bash
     "/home/{USERNAME}/.steam/steamapps/compatdata/{GRBL-STEAM-ID}/pfx/drive_c/Program Files (x86)/GRBL-Plotter/""
     ```

9. Then close the Properties Window and click on "Play". GRBL-Plotter will start like on Windows. Of course you can change the name of GRBL-Plotter_Setup.exe in the Properties Window.

## Setup USB Port to connect to the CNC

1. Connect your CNC via USB and check on which usb port is used
   
   ```bash
   ls /dev/ttyUSB*
   ```
   
   You should see and Output like `/dev/ttyUSB0` or `/dev/ttyACM0`
   
   Also check if the drivers are working in case you use a cheap ARDUINO with a CH340 Chip:
   
   ```bash
   lsusb
   ```

2. Open regedit for the GRBL-Plotter Proton installation
   
   ```bash
   protontricks {GRBL-STEAM-ID} regedit
   ```
   
   Add a string entry under HKEY_LOCAL_MACHINE\Software\Wine\Ports with a key of COM10 and a value of the USB port of the CNC like /dev/ttyUSB0 (or /dev/ttyACM0) to access the arduino USB port from wine and then create the symlink for it.

3. Now we have to create a new dosdevice to access the USB Port COM10 in GRBL-Potter
   
   ```bash
   ln -s /dev/ttyUSB0 ~/.steam/steam/steamapps/compatdata/{GRBL-PLOTTER-ID}/pfx/dosdevices/com10
   ```

4. Add yourself to dialout group so you can access the USB port
   
   ```bash
   sudo usermod -a -G dialout $USER
   ```

## Update GRBL-Plotter

To update GRBL-Plotter you can launch the new installer with wine and select the path to the proton prefix: 

```bash
/home/{USERNAME}/.steam/steamapps/compatdata/{GRBL-STEAM-ID}/pfx/drive_c/Program Files (x86)/GRBL-Plotter/
```
