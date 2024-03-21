# GRBL-Plotter

[![GitHub release](https://img.shields.io/github/release/svenhb/GRBL-Plotter.svg)](https://GitHub.com/svenhb/GRBL-Plotter/releases/)
[![GitHub latest commit](https://badgen.net/github/last-commit/svenhb/GRBL-Plotter)](https://GitHub.com/svenhb/GRBL-Plotter/commit/)
[![Github all releases](https://img.shields.io/github/downloads/svenhb/GRBL-Plotter/total.svg)](https://GitHub.com/svenhb/GRBL-Plotter/releases/)  

[README deutsch](README_de.md)  
A GCode sender for GRBL under windows, using DotNET 4.0 (should also work with Windows XP)  
使用 C# 和 VisualStudio 2022 编写   
Linux用户请查看这个链接 由metzger100编写: [GRBL-Plotter-Linux](https://github.com/svenhb/GRBL-Plotter/blob/master/doc/GRBL-Plotter-Linux.md)  


如果你喜欢 GRBL-Plotter, 可以向作者进行捐赠[![Donate](https://www.paypalobjects.com/en_US/DE/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=PVBK8U866QNQ6)   

### 查看 [历史](https://github.com/svenhb/GRBL-Plotter/blob/master/History.md)  

查看 [Wiki](https://github.com/svenhb/GRBL-Plotter/wiki) 获取更多信息   

### 本软件的免费的，使用过程中需要您自信承担可能带来的风险,<br>这里不对软件提供任何形式保证.
由VisualStudio生成的应用程序压缩包在以下目录: GRBL-Plotter/bin/release.
#### 官方网站：[GRBL-Plotter Vers. 1.7.3.0](https://github.com/svenhb/GRBL-Plotter/releases/latest)  2024-02-25     

### 编译依赖：
* VisualStudio 2022 
* DotNET 4.0

## 功能列表:
#### 导入/导出:  
* 控制落笔抬笔的选项
  - 控制Z轴
  - 控制舵机
  - 控制激光
  - 用户自定义命令
  - Create GCode absolute or relative (for further use as subroutine)  
* 标尺和导入文件的单位可以选择毫米(mm)和英寸(inch)
* 通过拖拽文件、复制文件导入Gcode
  - 加载Gcode时可以替换M3/M4指令 (对于"激光模式"有用 $32=1) 
* 通过拖拽文件、复制文件、URL导入SVG 图像,  - 使用 [Inkscape](https://inkscape.org/de/) 生成的SVG文件进行过测试 
  - 可选固定尺寸或变更尺寸
  - optional output of nodes only (generating drill holes for string art [Video 'String Art'](https://youtu.be/ymWi15rvTvM)  )
  - 可选通过颜色对生成的路径进行排序
  - 可以选择工具切换（换笔/换笔）
  - 如果需要添加文本需要先将文本转换为路径
* 通过拖拽文件、复制文件、URL导入DXF 图像-  使用  [LibreCAD](http://librecad.org/cms/home.html)  生成的DFX文件进行过测试
  - 有少量实体丢失
* [工具移动补偿](https://github.com/svenhb/GRBL-Plotter/wiki/Drag-tool-compensation)
* 通过拖拽文件、复制文件导入 HPGL 图像
* 通过拖拽文件、复制文件导入 Drill 文件
* 通过拖拽文件、复制文件导入 Gerber 文件(粗略的实现了这个功能) 
* 通过拖拽文件、复制文件导入 CSV 文件
* 通过拖拽文件、复制文件导入图片
* Gcode 可以编辑并保存
* 最近文件列表(支持文件和URL)
* 导入/导出机器的特定设置 (摇杆, 按钮)
  
#### G代码生成:
* 创建文本
  - 自己创建的'Dot Matrix' 字体 [Video 'Dot Matrix'](https://youtu.be/ip_qCQwoufw) 
* 创建简单的图形
* 创建条形码和二维码
* 通过工具扩展创建GCode
  
#### 导入选项: 
* 关联Z轴深度对应到笔宽
* 将圆半径加工为点（可选 Z 深度）
* 修改拖刀路径（用于纸张切割）
* 添加切向轴的角度信息
* 将剖面线填充添加到闭合路径
* 重复闭合路径一小段距离（用于激光切割）
* 按图形属性对代码进行分组：图层、颜色、画笔宽度
* 平铺图形

#### GCode 操作:  
* GCode 的转换（缩放、旋转、镜像、零偏移） 
任何 A、B、C、U、V、W 命令都将不被改变。
* 通过相机信息进行转换
* 旋转轴的轴替代
* 半径补偿

  
#### 机器控制:  
* 高度图、自动调平
* 探测平面
* 通过用户定义的按钮执行单独的命令
* 用户界面中的类似操纵杆的控件
* 支持杂牌 USB 游戏手柄/操纵杆
* 可选择使用具有独立坐标系的网络摄像头：当前 GCode 的图形叠加、设置零点、测量角度、缩放
  - 形状识别可更轻松地分配基准点
  - 使用相机辅助转换 GCode，以将钻孔文件与 PCB 外观相匹配 [Wiki 'PCB drilling'](https://github.com/svenhb/GRBL-Plotter/wiki/PCB-drilling)   
  
#### 流控:
* 支持子程序M98、M99子程序调用（P、L）
* Internal variable to support probing, e.g.:
  - G38.3 Z-50	（探头朝向刀具长度传感器，接触时停止 - 因为减速停止位置不是触发位置。）
  - G43.1 Z@PRBZ	（偏移工具的值存储在传感器开关的触发器上）
  - 参考 SerialForm.cs 来实现
* 通过电子邮件或推送通知进度通知
  
#### GRBL:  
* 程序启动时自动重新连接
* 支持 GRBL 1.1（也支持 0.9）
* 最大 30 kHz  步进引脚频率
* 支持新的 GRBL 1.1 功能
  - 手动移动
  - 进给率变速
  - 主轴变速
  - 实时显示GRBL状态（在COM CNC窗口）
* 检查 GRBL 设置的限制 - 最大值. 步进频率和最小值 COM CNC 窗口中的进给率
* 支持第 4 轴（A、B、C、U、V 或 W）。 状态和控制（需要特殊的GRBL版本）


### 我的测试机：
在我的德语主页上：
[my XYZ platform](http://svenhb.bplaced.net/?CNC___Plotter) 

### GRBL-Plotter 切换工具（换刀/换笔）
[![Import an image](https://i9.ytimg.com/vi/GGtdwYdZWi8/mq2.jpg?sqp=COypi98F&rs=AOn4CLAbkofKlCN1cepOQkGvpG6YlnRwrQ)](https://youtu.be/GGtdwYdZWi8) 

### 截图
主界面
![GRBL-Plotter GUI](doc/GRBLPlotter_GUI.png?raw=true "Main GUI") 

单独的串行 COM 窗口 - 一个用于 CNC，一个用于换刀装置（或第 4 轴）
![GRBL-Plotter COM interface](doc/GRBLPlotter_COM2.png?raw=true "Serial connection") ![2nd GRBL control](doc/GRBLPlotter_Control_COM2.png?raw=true "Serial connection")

导入/GCode 转换设置
![GRBL-Plotter Setup1.1](doc/screenshots/en_1325_setup_1_1.png?raw=true "Setup1.1") 
导入/GCode 转换设置![GRBL-Plotter Setup1.2](doc/screenshots/en_1325_setup_1_2.png?raw=true "Setup1.2") 
导入/GCode 转换设置
![GRBL-Plotter Setup1.3](doc/screenshots/en_1325_setup_1_3.png?raw=true "Setup1.3") 
导入/GCode 转换设置![GRBL-Plotter Setup1.4](doc/screenshots/en_1325_setup_1_4.png?raw=true "Setup1.4") 

工具表设置
![GRBL-Plotter Setup2](doc/screenshots/en_1325_setup_2.png?raw=true "Setup2")  

工具切换配置设置 
![GRBL-Plotter Setup3](doc/screenshots/en_1325_setup_3.png?raw=true "Setup3")  

流控设置 
![GRBL-Plotter Setup4](doc/screenshots/en_1325_setup_4.png?raw=true "Setup4")  

程序控制设置  
![GRBL-Plotter Setup5](doc/screenshots/en_1325_setup_5.png?raw=true "Setup5")  

Setup user defined buttons  
![GRBL-Plotter Setup6](doc/screenshots/en_1325_setup_6.png?raw=true "Setup6")  

游戏手柄设置  
![GRBL-Plotter Setup7](doc/screenshots/en_1325_setup_7.png?raw=true "Setup7")  

虚拟摇杆设置  
![GRBL-Plotter Setup8](doc/screenshots/en_1325_setup_8.png?raw=true "Setup8")  

快捷键设置     
![GRBL-Plotter Setup9](doc/screenshots/en_1325_setup_9.png?raw=true "Setup9")  

基准相机的形状识别设置    
![GRBL-Plotter Setup10](doc/screenshots/en_1325_setup_10.png?raw=true "Setup10")  
    
导入文字  
![GRBL-Plotter Text](doc/GRBLPlotter_Text.png?raw=true "Text conversion")  

导入图片  
![GRBL-Plotter Image](doc/ImageImport/ImageImport1.png?raw=true "Image import")  

不同的缩放选项  
![GRBL-Plotter Scaling](doc/GRBLPlotter_scaling.png?raw=true "GCode scaling")  

版本 0.9 和 1.1 的进给速率倍率  
![GRBL-Plotter Override](doc/GRBLPlotter_override.png?raw=true "GCode override") ![GRBL-Plotter Override](doc/GRBLPlotter_override2.png?raw=true "GCode override")
