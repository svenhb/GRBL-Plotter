/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2020 Sven Hasemann contact: svenhb@web.de

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
namespace GRBL_Plotter
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                picBoxBackround.Dispose();
                StyleComment.Dispose();
                StyleFWord.Dispose();
                StyleGWord.Dispose();
                StyleMWord.Dispose();
                StyleSWord.Dispose();
                StyleTool.Dispose();
                StyleXAxis.Dispose();
                StyleYAxis.Dispose();
                StyleZAxis.Dispose();
                StyleCommentxml.Dispose();
                StyleFail.Dispose();

                penUp.Dispose();
                penDown.Dispose();
                penRotary.Dispose();
                penHeightMap.Dispose();
                penRuler.Dispose();
                penTool.Dispose();
                penMarker.Dispose();
                penLandMark.Dispose();
                penSimulation.Dispose();

                brushMachineLimit.Dispose();
                brushBackground.Dispose();
                StyleAAxis.Dispose();
                StyleLineN.Dispose();
                pBoxOrig.Dispose();
                pBoxTransform.Dispose();

                ErrorStyle.Dispose();
                StyleTT.Dispose();
                Style2nd.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tLPLinks = new System.Windows.Forms.TableLayoutPanel();
            this.fCTBCode = new FastColoredTextBoxNS.FastColoredTextBox();
            this.cmsFCTB = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unDo3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsEditorHotkeys = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsCodeBlocksFold = new System.Windows.Forms.ToolStripMenuItem();
            this.foldCodeBlocks1stLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldCodeBlocks2ndLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foldCodeBlocks3rdLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandCodeBlocksToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksMove = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFCTBMoveSelectedCodeBlockMostUp = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFCTBMoveSelectedCodeBlockUp = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFCTBMoveSelectedCodeBlockDown = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsFCTBMoveSelectedCodeBlockMostDown = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSort = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortReverse = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortById = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByColor = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByPenWidthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByLayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortByTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByGeometry = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByToolNr = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByToolName = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByCodeSize = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByCodeArea = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksSortByDistance = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksRemoveGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeBlocksRemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsEditMode = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodeCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodePaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodePasteSpecial1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCodePasteSpecial2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsFindDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsReplaceDialog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsCodeSendLine = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCommentOut = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsUpdate2DView = new System.Windows.Forms.ToolStripMenuItem();
            this.gBoxStream = new System.Windows.Forms.GroupBox();
            this.btnSimulatePause = new System.Windows.Forms.Button();
            this.btnSimulateSlower = new System.Windows.Forms.Button();
            this.btnSimulateFaster = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.lbInfo = new System.Windows.Forms.Label();
            this.pbBuffer = new System.Windows.Forms.ProgressBar();
            this.btnStreamStop = new System.Windows.Forms.Button();
            this.btnStreamCheck = new System.Windows.Forms.Button();
            this.lblRemaining = new System.Windows.Forms.Label();
            this.pbFile = new System.Windows.Forms.ProgressBar();
            this.lblElapsed = new System.Windows.Forms.Label();
            this.lblFileProgress = new System.Windows.Forms.Label();
            this.btnStreamStart = new System.Windows.Forms.Button();
            this.tbFile = new System.Windows.Forms.TextBox();
            this.gBoxDimension = new System.Windows.Forms.GroupBox();
            this.btnLimitExceed = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnOffsetApply = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.tbOffsetY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbOffsetX = new System.Windows.Forms.TextBox();
            this.rBOrigin9 = new System.Windows.Forms.RadioButton();
            this.rBOrigin8 = new System.Windows.Forms.RadioButton();
            this.rBOrigin7 = new System.Windows.Forms.RadioButton();
            this.rBOrigin6 = new System.Windows.Forms.RadioButton();
            this.rBOrigin5 = new System.Windows.Forms.RadioButton();
            this.rBOrigin4 = new System.Windows.Forms.RadioButton();
            this.rBOrigin3 = new System.Windows.Forms.RadioButton();
            this.rBOrigin2 = new System.Windows.Forms.RadioButton();
            this.rBOrigin1 = new System.Windows.Forms.RadioButton();
            this.lbDimension = new System.Windows.Forms.TextBox();
            this.gBoxOverride = new System.Windows.Forms.GroupBox();
            this.gBOverrideRGB = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnOverrideRapid0 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btnOverrideRapid2 = new System.Windows.Forms.Button();
            this.lblOverrideRapidValue = new System.Windows.Forms.Label();
            this.btnOverrideRapid1 = new System.Windows.Forms.Button();
            this.gBOverrideASGB = new System.Windows.Forms.GroupBox();
            this.btnOverrideSpindle = new System.Windows.Forms.Button();
            this.btnOverrideMist = new System.Windows.Forms.Button();
            this.btnOverrideFlood = new System.Windows.Forms.Button();
            this.gBOverrideFRGB = new System.Windows.Forms.GroupBox();
            this.lblStatusFeed = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblOverrideFRValue = new System.Windows.Forms.Label();
            this.btnOverrideFR1 = new System.Windows.Forms.Button();
            this.btnOverrideFR2 = new System.Windows.Forms.Button();
            this.btnOverrideFR0 = new System.Windows.Forms.Button();
            this.btnOverrideFR4 = new System.Windows.Forms.Button();
            this.btnOverrideFR3 = new System.Windows.Forms.Button();
            this.gBOverrideSSGB = new System.Windows.Forms.GroupBox();
            this.lblStatusSpeed = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblOverrideSSValue = new System.Windows.Forms.Label();
            this.btnOverrideSS2 = new System.Windows.Forms.Button();
            this.btnOverrideSS0 = new System.Windows.Forms.Button();
            this.btnOverrideSS1 = new System.Windows.Forms.Button();
            this.btnOverrideSS4 = new System.Windows.Forms.Button();
            this.btnOverrideSS3 = new System.Windows.Forms.Button();
            this.tLPRechts = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUntenRechts = new System.Windows.Forms.TableLayoutPanel();
            this.gB_Jogging = new System.Windows.Forms.GroupBox();
            this.cBServoButtons = new System.Windows.Forms.CheckBox();
            this.btnPenDown = new System.Windows.Forms.Button();
            this.btnPenUp = new System.Windows.Forms.Button();
            this.btnJogStop = new System.Windows.Forms.Button();
            this.gB_Jog0 = new System.Windows.Forms.GroupBox();
            this.btnJogZeroA = new System.Windows.Forms.Button();
            this.btnJogZeroX = new System.Windows.Forms.Button();
            this.btnJogZeroXY = new System.Windows.Forms.Button();
            this.btnJogZeroY = new System.Windows.Forms.Button();
            this.btnJogZeroZ = new System.Windows.Forms.Button();
            this.cBMoveG0 = new System.Windows.Forms.CheckBox();
            this.lblTool = new System.Windows.Forms.Label();
            this.cBTool = new System.Windows.Forms.CheckBox();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.tBSpeed = new System.Windows.Forms.TextBox();
            this.cBCoolant = new System.Windows.Forms.CheckBox();
            this.cBSpindle = new System.Windows.Forms.CheckBox();
            this.tLPRechtsUntenRechtsMitte = new System.Windows.Forms.TableLayoutPanel();
            this.virtualJoystickA = new virtualJoystick.virtualJoystick();
            this.virtualJoystickXY = new virtualJoystick.virtualJoystick();
            this.virtualJoystickZ = new virtualJoystick.virtualJoystick();
            this.virtualJoystickB = new virtualJoystick.virtualJoystick();
            this.virtualJoystickC = new virtualJoystick.virtualJoystick();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.cBSendJogStop = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOverrideDoor = new System.Windows.Forms.Button();
            this.btnResume = new System.Windows.Forms.Button();
            this.btnFeedHold = new System.Windows.Forms.Button();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnKillAlarm = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tLPMitteUnten = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmsPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unDo2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxMoveToMarkedPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxZeroXYAtMarkedPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxMoveGraphicsOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxResetZooming = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxPasteFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxReloadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.deletenotMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxMarkFirstPos = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxDeletePath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxCropSelectedPath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxMoveSelectedPathInCode = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxReverseSelectedPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxSetGCodeAsBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxClearBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.copyContentTroClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tBURL = new System.Windows.Forms.TextBox();
            this.tLPRechtsOben = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tLPCustomButton1 = new System.Windows.Forms.TableLayoutPanel();
            this.tLPCustomButton2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCustom1 = new System.Windows.Forms.Button();
            this.btnCustom2 = new System.Windows.Forms.Button();
            this.btnCustom3 = new System.Windows.Forms.Button();
            this.btnCustom4 = new System.Windows.Forms.Button();
            this.btnCustom5 = new System.Windows.Forms.Button();
            this.btnCustom6 = new System.Windows.Forms.Button();
            this.btnCustom7 = new System.Windows.Forms.Button();
            this.btnCustom8 = new System.Windows.Forms.Button();
            this.btnCustom9 = new System.Windows.Forms.Button();
            this.btnCustom10 = new System.Windows.Forms.Button();
            this.btnCustom11 = new System.Windows.Forms.Button();
            this.btnCustom12 = new System.Windows.Forms.Button();
            this.btnCustom13 = new System.Windows.Forms.Button();
            this.btnCustom14 = new System.Windows.Forms.Button();
            this.btnCustom15 = new System.Windows.Forms.Button();
            this.btnCustom16 = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBoxCoordinates = new System.Windows.Forms.GroupBox();
            this.label_c = new System.Windows.Forms.Label();
            this.btnZeroC = new System.Windows.Forms.Button();
            this.label_mc = new System.Windows.Forms.Label();
            this.label_wc = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.btnZeroB = new System.Windows.Forms.Button();
            this.label_mb = new System.Windows.Forms.Label();
            this.label_wb = new System.Windows.Forms.Label();
            this.lblCurrentG = new System.Windows.Forms.Label();
            this.label_status0 = new System.Windows.Forms.Label();
            this.label_a = new System.Windows.Forms.Label();
            this.btnZeroA = new System.Windows.Forms.Button();
            this.label_ma = new System.Windows.Forms.Label();
            this.label_wa = new System.Windows.Forms.Label();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnZeroXYZ = new System.Windows.Forms.Button();
            this.btnZeroXY = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.btnZeroZ = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.btnZeroY = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnZeroX = new System.Windows.Forms.Button();
            this.label_status = new System.Windows.Forms.Label();
            this.label_mx = new System.Windows.Forms.Label();
            this.label_my = new System.Windows.Forms.Label();
            this.label_mz = new System.Windows.Forms.Label();
            this.label_wz = new System.Windows.Forms.Label();
            this.label_wx = new System.Windows.Forms.Label();
            this.label_wy = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.setupToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.saveMachineParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMachineParametersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deutschToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pусскийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.portuguêsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.franzToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chinesischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arabischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.japanischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textWizzardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBarcodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createSimpleShapesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createJogPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startExtensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unDoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.mirrorXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mirrorRotaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.rotate90ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate90ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.rotate180ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateFreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_rotate = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.sToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_XY_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skalierenXYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_XY_X_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skalierenXYUmXUnitsZuErreichenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_XY_Y_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereXUmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_X_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereAufXUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_X_X_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereYUmToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_Y_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereAufYUnitsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_Y_Y_scale = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.rotaryDimaeterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_rotary_diameter = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereXAufDrehachseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_X_A_scale = new System.Windows.Forms.ToolStripTextBox();
            this.skaliereYAufDrehachseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_Y_A_scale = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStrip_RadiusComp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tBRadiusCompValue = new System.Windows.Forms.ToolStripTextBox();
            this.ersetzteG23DurchLinienToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertZToSspindleSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeAnyZMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.workpieceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.probingToolLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.surfaceScanHeightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.jogPathCreatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.machineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.edgeFinderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.heightMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.laserToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.coordinateSystemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.startStreamingAtLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip_tb_StreamLine = new System.Windows.Forms.ToolStripTextBox();
            this.controlStreamingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.control2ndGRBLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.control3rdGRBLToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewRuler = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewPenUp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewMachineFix = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewMachine = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewDimension = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewTool = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripViewBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.gamePadTimer = new System.Windows.Forms.Timer(this.components);
            this.simulationTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel0 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplashScreenTimer = new System.Windows.Forms.Timer(this.components);
            this.loadTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tLPLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).BeginInit();
            this.cmsFCTB.SuspendLayout();
            this.gBoxStream.SuspendLayout();
            this.gBoxDimension.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.gBoxOverride.SuspendLayout();
            this.gBOverrideRGB.SuspendLayout();
            this.gBOverrideASGB.SuspendLayout();
            this.gBOverrideFRGB.SuspendLayout();
            this.gBOverrideSSGB.SuspendLayout();
            this.tLPRechts.SuspendLayout();
            this.tLPRechtsUnten.SuspendLayout();
            this.tLPRechtsUntenRechts.SuspendLayout();
            this.gB_Jogging.SuspendLayout();
            this.gB_Jog0.SuspendLayout();
            this.tLPRechtsUntenRechtsMitte.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tLPMitteUnten.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.cmsPictureBox.SuspendLayout();
            this.tLPRechtsOben.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tLPCustomButton1.SuspendLayout();
            this.tLPCustomButton2.SuspendLayout();
            this.groupBoxCoordinates.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tLPLinks);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tLPRechts);
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // tLPLinks
            // 
            resources.ApplyResources(this.tLPLinks, "tLPLinks");
            this.tLPLinks.Controls.Add(this.fCTBCode, 0, 3);
            this.tLPLinks.Controls.Add(this.gBoxStream, 0, 0);
            this.tLPLinks.Controls.Add(this.gBoxDimension, 0, 2);
            this.tLPLinks.Controls.Add(this.gBoxOverride, 0, 1);
            this.tLPLinks.Name = "tLPLinks";
            // 
            // fCTBCode
            // 
            this.fCTBCode.AllowMacroRecording = false;
            this.fCTBCode.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fCTBCode.AutoIndent = false;
            this.fCTBCode.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+\\s*(?<range>=)\\s*(?<range>[^;]+);";
            resources.ApplyResources(this.fCTBCode, "fCTBCode");
            this.fCTBCode.BackBrush = null;
            this.fCTBCode.CharCnWidth = 13;
            this.fCTBCode.CharHeight = 12;
            this.fCTBCode.CharWidth = 7;
            this.fCTBCode.ContextMenuStrip = this.cmsFCTB;
            this.fCTBCode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.fCTBCode.DelayedTextChangedInterval = 200;
            this.fCTBCode.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fCTBCode.Hotkeys = resources.GetString("fCTBCode.Hotkeys");
            this.fCTBCode.IsReplaceMode = false;
            this.fCTBCode.Name = "fCTBCode";
            this.fCTBCode.Paddings = new System.Windows.Forms.Padding(0);
            this.fCTBCode.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.fCTBCode.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("fCTBCode.ServiceColors")));
            this.fCTBCode.ShowFoldingLines = true;
            this.fCTBCode.ToolTip = null;
            this.fCTBCode.Zoom = 100;
            this.fCTBCode.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChanged);
            this.fCTBCode.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fCTBCode_TextChangedDelayed);
            this.fCTBCode.Click += new System.EventHandler(this.fCTBCode_Click);
            this.fCTBCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fCTBCode_KeyDown);
            this.fCTBCode.MouseHover += new System.EventHandler(this.fCTBCode_MouseHover);
            // 
            // cmsFCTB
            // 
            this.cmsFCTB.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unDo3ToolStripMenuItem,
            this.toolStripSeparator13,
            this.cmsEditorHotkeys,
            this.toolStripSeparator19,
            this.cmsCodeBlocksFold,
            this.cmsCodeBlocksMove,
            this.cmsCodeBlocksSort,
            this.cmsCodeBlocksRemoveGroup,
            this.cmsCodeBlocksRemoveAll,
            this.toolStripSeparator11,
            this.cmsEditMode,
            this.cmsCodeSelect,
            this.cmsCodeCopy,
            this.cmsCodePaste,
            this.cmsCodePasteSpecial1,
            this.cmsCodePasteSpecial2,
            this.toolStripSeparator14,
            this.cmsFindDialog,
            this.cmsReplaceDialog,
            this.toolStripSeparator12,
            this.cmsCodeSendLine,
            this.cmsCommentOut,
            this.cmsUpdate2DView});
            this.cmsFCTB.Name = "cmsCode";
            this.cmsFCTB.ShowImageMargin = false;
            resources.ApplyResources(this.cmsFCTB, "cmsFCTB");
            this.cmsFCTB.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsFCTB_ItemClicked);
            // 
            // unDo3ToolStripMenuItem
            // 
            this.unDo3ToolStripMenuItem.Name = "unDo3ToolStripMenuItem";
            resources.ApplyResources(this.unDo3ToolStripMenuItem, "unDo3ToolStripMenuItem");
            this.unDo3ToolStripMenuItem.Click += new System.EventHandler(this.unDoToolStripMenuItem_Click);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // cmsEditorHotkeys
            // 
            resources.ApplyResources(this.cmsEditorHotkeys, "cmsEditorHotkeys");
            this.cmsEditorHotkeys.Name = "cmsEditorHotkeys";
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // cmsCodeBlocksFold
            // 
            resources.ApplyResources(this.cmsCodeBlocksFold, "cmsCodeBlocksFold");
            this.cmsCodeBlocksFold.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldCodeBlocks1stLevelToolStripMenuItem,
            this.foldCodeBlocks2ndLevelToolStripMenuItem,
            this.foldCodeBlocks3rdLevelToolStripMenuItem,
            this.expandCodeBlocksToolStripMenuItem1});
            this.cmsCodeBlocksFold.Name = "cmsCodeBlocksFold";
            // 
            // foldCodeBlocks1stLevelToolStripMenuItem
            // 
            this.foldCodeBlocks1stLevelToolStripMenuItem.Name = "foldCodeBlocks1stLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks1stLevelToolStripMenuItem, "foldCodeBlocks1stLevelToolStripMenuItem");
            this.foldCodeBlocks1stLevelToolStripMenuItem.Click += new System.EventHandler(this.foldBlocks1stToolStripMenuItem1_Click);
            // 
            // foldCodeBlocks2ndLevelToolStripMenuItem
            // 
            this.foldCodeBlocks2ndLevelToolStripMenuItem.Name = "foldCodeBlocks2ndLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks2ndLevelToolStripMenuItem, "foldCodeBlocks2ndLevelToolStripMenuItem");
            this.foldCodeBlocks2ndLevelToolStripMenuItem.Click += new System.EventHandler(this.foldBlocks2ndToolStripMenuItem1_Click);
            // 
            // foldCodeBlocks3rdLevelToolStripMenuItem
            // 
            this.foldCodeBlocks3rdLevelToolStripMenuItem.Name = "foldCodeBlocks3rdLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks3rdLevelToolStripMenuItem, "foldCodeBlocks3rdLevelToolStripMenuItem");
            this.foldCodeBlocks3rdLevelToolStripMenuItem.Click += new System.EventHandler(this.foldBlocks3rdToolStripMenuItem1_Click);
            // 
            // expandCodeBlocksToolStripMenuItem1
            // 
            this.expandCodeBlocksToolStripMenuItem1.Name = "expandCodeBlocksToolStripMenuItem1";
            resources.ApplyResources(this.expandCodeBlocksToolStripMenuItem1, "expandCodeBlocksToolStripMenuItem1");
            this.expandCodeBlocksToolStripMenuItem1.Click += new System.EventHandler(this.expandCodeBlocksToolStripMenuItem_Click);
            // 
            // cmsCodeBlocksMove
            // 
            resources.ApplyResources(this.cmsCodeBlocksMove, "cmsCodeBlocksMove");
            this.cmsCodeBlocksMove.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsFCTBMoveSelectedCodeBlockMostUp,
            this.cmsFCTBMoveSelectedCodeBlockUp,
            this.cmsFCTBMoveSelectedCodeBlockDown,
            this.cmsFCTBMoveSelectedCodeBlockMostDown});
            this.cmsCodeBlocksMove.Name = "cmsCodeBlocksMove";
            // 
            // cmsFCTBMoveSelectedCodeBlockMostUp
            // 
            this.cmsFCTBMoveSelectedCodeBlockMostUp.Name = "cmsFCTBMoveSelectedCodeBlockMostUp";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockMostUp, "cmsFCTBMoveSelectedCodeBlockMostUp");
            this.cmsFCTBMoveSelectedCodeBlockMostUp.Click += new System.EventHandler(this.moveSelectedCodeBlockMostUpToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockUp
            // 
            this.cmsFCTBMoveSelectedCodeBlockUp.Name = "cmsFCTBMoveSelectedCodeBlockUp";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockUp, "cmsFCTBMoveSelectedCodeBlockUp");
            this.cmsFCTBMoveSelectedCodeBlockUp.Click += new System.EventHandler(this.moveSelectedCodeBlockUpToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockDown
            // 
            this.cmsFCTBMoveSelectedCodeBlockDown.Name = "cmsFCTBMoveSelectedCodeBlockDown";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockDown, "cmsFCTBMoveSelectedCodeBlockDown");
            this.cmsFCTBMoveSelectedCodeBlockDown.Click += new System.EventHandler(this.moveSelectedCodeBlockDownToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockMostDown
            // 
            this.cmsFCTBMoveSelectedCodeBlockMostDown.Name = "cmsFCTBMoveSelectedCodeBlockMostDown";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockMostDown, "cmsFCTBMoveSelectedCodeBlockMostDown");
            this.cmsFCTBMoveSelectedCodeBlockMostDown.Click += new System.EventHandler(this.moveSelectedCodeBlockMostDownToolStripMenuItem_Click);
            // 
            // cmsCodeBlocksSort
            // 
            this.cmsCodeBlocksSort.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsCodeBlocksSortReverse,
            this.cmsCodeBlocksSortById,
            this.cmsCodeBlocksSortByColor,
            this.sortByPenWidthToolStripMenuItem,
            this.sortByLayerToolStripMenuItem,
            this.sortByTypeToolStripMenuItem,
            this.cmsCodeBlocksSortByGeometry,
            this.cmsCodeBlocksSortByToolNr,
            this.cmsCodeBlocksSortByToolName,
            this.cmsCodeBlocksSortByCodeSize,
            this.cmsCodeBlocksSortByCodeArea,
            this.cmsCodeBlocksSortByDistance});
            resources.ApplyResources(this.cmsCodeBlocksSort, "cmsCodeBlocksSort");
            this.cmsCodeBlocksSort.Name = "cmsCodeBlocksSort";
            // 
            // cmsCodeBlocksSortReverse
            // 
            this.cmsCodeBlocksSortReverse.CheckOnClick = true;
            this.cmsCodeBlocksSortReverse.Name = "cmsCodeBlocksSortReverse";
            resources.ApplyResources(this.cmsCodeBlocksSortReverse, "cmsCodeBlocksSortReverse");
            // 
            // cmsCodeBlocksSortById
            // 
            this.cmsCodeBlocksSortById.Name = "cmsCodeBlocksSortById";
            resources.ApplyResources(this.cmsCodeBlocksSortById, "cmsCodeBlocksSortById");
            this.cmsCodeBlocksSortById.Click += new System.EventHandler(this.cmsCodeBlocksSortById_Click);
            // 
            // cmsCodeBlocksSortByColor
            // 
            this.cmsCodeBlocksSortByColor.Name = "cmsCodeBlocksSortByColor";
            resources.ApplyResources(this.cmsCodeBlocksSortByColor, "cmsCodeBlocksSortByColor");
            this.cmsCodeBlocksSortByColor.Click += new System.EventHandler(this.cmsCodeBlocksSortByColor_Click);
            // 
            // sortByPenWidthToolStripMenuItem
            // 
            this.sortByPenWidthToolStripMenuItem.Name = "sortByPenWidthToolStripMenuItem";
            resources.ApplyResources(this.sortByPenWidthToolStripMenuItem, "sortByPenWidthToolStripMenuItem");
            this.sortByPenWidthToolStripMenuItem.Click += new System.EventHandler(this.cmsCodeBlocksSortByWidth_Click);
            // 
            // sortByLayerToolStripMenuItem
            // 
            this.sortByLayerToolStripMenuItem.Name = "sortByLayerToolStripMenuItem";
            resources.ApplyResources(this.sortByLayerToolStripMenuItem, "sortByLayerToolStripMenuItem");
            this.sortByLayerToolStripMenuItem.Click += new System.EventHandler(this.cmsCodeBlocksSortByLayer_Click);
            // 
            // sortByTypeToolStripMenuItem
            // 
            this.sortByTypeToolStripMenuItem.Name = "sortByTypeToolStripMenuItem";
            resources.ApplyResources(this.sortByTypeToolStripMenuItem, "sortByTypeToolStripMenuItem");
            this.sortByTypeToolStripMenuItem.Click += new System.EventHandler(this.cmsCodeBlocksSortByType_Click);
            // 
            // cmsCodeBlocksSortByGeometry
            // 
            this.cmsCodeBlocksSortByGeometry.Name = "cmsCodeBlocksSortByGeometry";
            resources.ApplyResources(this.cmsCodeBlocksSortByGeometry, "cmsCodeBlocksSortByGeometry");
            this.cmsCodeBlocksSortByGeometry.Click += new System.EventHandler(this.cmsCodeBlocksSortByGeometry_Click);
            // 
            // cmsCodeBlocksSortByToolNr
            // 
            this.cmsCodeBlocksSortByToolNr.Name = "cmsCodeBlocksSortByToolNr";
            resources.ApplyResources(this.cmsCodeBlocksSortByToolNr, "cmsCodeBlocksSortByToolNr");
            this.cmsCodeBlocksSortByToolNr.Click += new System.EventHandler(this.cmsCodeBlocksSortByToolNr_Click);
            // 
            // cmsCodeBlocksSortByToolName
            // 
            this.cmsCodeBlocksSortByToolName.Name = "cmsCodeBlocksSortByToolName";
            resources.ApplyResources(this.cmsCodeBlocksSortByToolName, "cmsCodeBlocksSortByToolName");
            this.cmsCodeBlocksSortByToolName.Click += new System.EventHandler(this.cmsCodeBlocksSortByToolName_Click);
            // 
            // cmsCodeBlocksSortByCodeSize
            // 
            this.cmsCodeBlocksSortByCodeSize.Name = "cmsCodeBlocksSortByCodeSize";
            resources.ApplyResources(this.cmsCodeBlocksSortByCodeSize, "cmsCodeBlocksSortByCodeSize");
            this.cmsCodeBlocksSortByCodeSize.Click += new System.EventHandler(this.cmsCodeBlocksSortByCodeSize_Click);
            // 
            // cmsCodeBlocksSortByCodeArea
            // 
            this.cmsCodeBlocksSortByCodeArea.Name = "cmsCodeBlocksSortByCodeArea";
            resources.ApplyResources(this.cmsCodeBlocksSortByCodeArea, "cmsCodeBlocksSortByCodeArea");
            this.cmsCodeBlocksSortByCodeArea.Click += new System.EventHandler(this.cmsCodeBlocksSortByCodeArea_Click);
            // 
            // cmsCodeBlocksSortByDistance
            // 
            this.cmsCodeBlocksSortByDistance.Name = "cmsCodeBlocksSortByDistance";
            resources.ApplyResources(this.cmsCodeBlocksSortByDistance, "cmsCodeBlocksSortByDistance");
            this.cmsCodeBlocksSortByDistance.Click += new System.EventHandler(this.cmsCodeBlocksSortByDistance_Click);
            // 
            // cmsCodeBlocksRemoveGroup
            // 
            resources.ApplyResources(this.cmsCodeBlocksRemoveGroup, "cmsCodeBlocksRemoveGroup");
            this.cmsCodeBlocksRemoveGroup.Name = "cmsCodeBlocksRemoveGroup";
            this.cmsCodeBlocksRemoveGroup.Click += new System.EventHandler(this.cmsCodeBlocksRemoveGroup_Click);
            // 
            // cmsCodeBlocksRemoveAll
            // 
            resources.ApplyResources(this.cmsCodeBlocksRemoveAll, "cmsCodeBlocksRemoveAll");
            this.cmsCodeBlocksRemoveAll.Name = "cmsCodeBlocksRemoveAll";
            this.cmsCodeBlocksRemoveAll.Click += new System.EventHandler(this.cmsCodeBlocksRemoveAll_Click);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // cmsEditMode
            // 
            resources.ApplyResources(this.cmsEditMode, "cmsEditMode");
            this.cmsEditMode.Name = "cmsEditMode";
            // 
            // cmsCodeSelect
            // 
            resources.ApplyResources(this.cmsCodeSelect, "cmsCodeSelect");
            this.cmsCodeSelect.Name = "cmsCodeSelect";
            // 
            // cmsCodeCopy
            // 
            resources.ApplyResources(this.cmsCodeCopy, "cmsCodeCopy");
            this.cmsCodeCopy.Name = "cmsCodeCopy";
            // 
            // cmsCodePaste
            // 
            resources.ApplyResources(this.cmsCodePaste, "cmsCodePaste");
            this.cmsCodePaste.Name = "cmsCodePaste";
            // 
            // cmsCodePasteSpecial1
            // 
            resources.ApplyResources(this.cmsCodePasteSpecial1, "cmsCodePasteSpecial1");
            this.cmsCodePasteSpecial1.Name = "cmsCodePasteSpecial1";
            // 
            // cmsCodePasteSpecial2
            // 
            resources.ApplyResources(this.cmsCodePasteSpecial2, "cmsCodePasteSpecial2");
            this.cmsCodePasteSpecial2.Name = "cmsCodePasteSpecial2";
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // cmsFindDialog
            // 
            resources.ApplyResources(this.cmsFindDialog, "cmsFindDialog");
            this.cmsFindDialog.Name = "cmsFindDialog";
            // 
            // cmsReplaceDialog
            // 
            resources.ApplyResources(this.cmsReplaceDialog, "cmsReplaceDialog");
            this.cmsReplaceDialog.Name = "cmsReplaceDialog";
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // cmsCodeSendLine
            // 
            this.cmsCodeSendLine.Name = "cmsCodeSendLine";
            resources.ApplyResources(this.cmsCodeSendLine, "cmsCodeSendLine");
            // 
            // cmsCommentOut
            // 
            this.cmsCommentOut.Name = "cmsCommentOut";
            resources.ApplyResources(this.cmsCommentOut, "cmsCommentOut");
            // 
            // cmsUpdate2DView
            // 
            resources.ApplyResources(this.cmsUpdate2DView, "cmsUpdate2DView");
            this.cmsUpdate2DView.Name = "cmsUpdate2DView";
            // 
            // gBoxStream
            // 
            this.gBoxStream.Controls.Add(this.btnSimulatePause);
            this.gBoxStream.Controls.Add(this.btnSimulateSlower);
            this.gBoxStream.Controls.Add(this.btnSimulateFaster);
            this.gBoxStream.Controls.Add(this.btnSimulate);
            this.gBoxStream.Controls.Add(this.lbInfo);
            this.gBoxStream.Controls.Add(this.pbBuffer);
            this.gBoxStream.Controls.Add(this.btnStreamStop);
            this.gBoxStream.Controls.Add(this.btnStreamCheck);
            this.gBoxStream.Controls.Add(this.lblRemaining);
            this.gBoxStream.Controls.Add(this.pbFile);
            this.gBoxStream.Controls.Add(this.lblElapsed);
            this.gBoxStream.Controls.Add(this.lblFileProgress);
            this.gBoxStream.Controls.Add(this.btnStreamStart);
            this.gBoxStream.Controls.Add(this.tbFile);
            resources.ApplyResources(this.gBoxStream, "gBoxStream");
            this.gBoxStream.Name = "gBoxStream";
            this.gBoxStream.TabStop = false;
            // 
            // btnSimulatePause
            // 
            this.btnSimulatePause.Image = global::GRBL_Plotter.Properties.Resources.btn_pause;
            resources.ApplyResources(this.btnSimulatePause, "btnSimulatePause");
            this.btnSimulatePause.Name = "btnSimulatePause";
            this.btnSimulatePause.UseVisualStyleBackColor = true;
            this.btnSimulatePause.Click += new System.EventHandler(this.btnSimulatePause_Click);
            // 
            // btnSimulateSlower
            // 
            resources.ApplyResources(this.btnSimulateSlower, "btnSimulateSlower");
            this.btnSimulateSlower.Name = "btnSimulateSlower";
            this.toolTip1.SetToolTip(this.btnSimulateSlower, resources.GetString("btnSimulateSlower.ToolTip"));
            this.btnSimulateSlower.UseVisualStyleBackColor = true;
            this.btnSimulateSlower.Click += new System.EventHandler(this.btnSimulateSlower_Click);
            // 
            // btnSimulateFaster
            // 
            resources.ApplyResources(this.btnSimulateFaster, "btnSimulateFaster");
            this.btnSimulateFaster.Name = "btnSimulateFaster";
            this.toolTip1.SetToolTip(this.btnSimulateFaster, resources.GetString("btnSimulateFaster.ToolTip"));
            this.btnSimulateFaster.UseVisualStyleBackColor = true;
            this.btnSimulateFaster.Click += new System.EventHandler(this.btnSimulateFaster_Click);
            // 
            // btnSimulate
            // 
            resources.ApplyResources(this.btnSimulate, "btnSimulate");
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Click += new System.EventHandler(this.btnSimulate_Click);
            // 
            // lbInfo
            // 
            resources.ApplyResources(this.lbInfo, "lbInfo");
            this.lbInfo.Name = "lbInfo";
            // 
            // pbBuffer
            // 
            resources.ApplyResources(this.pbBuffer, "pbBuffer");
            this.pbBuffer.Name = "pbBuffer";
            // 
            // btnStreamStop
            // 
            this.btnStreamStop.Image = global::GRBL_Plotter.Properties.Resources.btn_stop;
            resources.ApplyResources(this.btnStreamStop, "btnStreamStop");
            this.btnStreamStop.Name = "btnStreamStop";
            this.toolTip1.SetToolTip(this.btnStreamStop, resources.GetString("btnStreamStop.ToolTip"));
            this.btnStreamStop.UseVisualStyleBackColor = true;
            this.btnStreamStop.Click += new System.EventHandler(this.btnStreamStop_Click);
            // 
            // btnStreamCheck
            // 
            resources.ApplyResources(this.btnStreamCheck, "btnStreamCheck");
            this.btnStreamCheck.Name = "btnStreamCheck";
            this.toolTip1.SetToolTip(this.btnStreamCheck, resources.GetString("btnStreamCheck.ToolTip"));
            this.btnStreamCheck.UseVisualStyleBackColor = true;
            this.btnStreamCheck.Click += new System.EventHandler(this.btnStreamCheck_Click);
            // 
            // lblRemaining
            // 
            resources.ApplyResources(this.lblRemaining, "lblRemaining");
            this.lblRemaining.Name = "lblRemaining";
            // 
            // pbFile
            // 
            resources.ApplyResources(this.pbFile, "pbFile");
            this.pbFile.Name = "pbFile";
            // 
            // lblElapsed
            // 
            resources.ApplyResources(this.lblElapsed, "lblElapsed");
            this.lblElapsed.Name = "lblElapsed";
            // 
            // lblFileProgress
            // 
            resources.ApplyResources(this.lblFileProgress, "lblFileProgress");
            this.lblFileProgress.Name = "lblFileProgress";
            // 
            // btnStreamStart
            // 
            this.btnStreamStart.Image = global::GRBL_Plotter.Properties.Resources.btn_play;
            resources.ApplyResources(this.btnStreamStart, "btnStreamStart");
            this.btnStreamStart.Name = "btnStreamStart";
            this.toolTip1.SetToolTip(this.btnStreamStart, resources.GetString("btnStreamStart.ToolTip"));
            this.btnStreamStart.UseVisualStyleBackColor = true;
            this.btnStreamStart.Click += new System.EventHandler(this.btnStreamStart_Click);
            // 
            // tbFile
            // 
            resources.ApplyResources(this.tbFile, "tbFile");
            this.tbFile.Name = "tbFile";
            // 
            // gBoxDimension
            // 
            this.gBoxDimension.Controls.Add(this.btnLimitExceed);
            this.gBoxDimension.Controls.Add(this.groupBox4);
            this.gBoxDimension.Controls.Add(this.lbDimension);
            resources.ApplyResources(this.gBoxDimension, "gBoxDimension");
            this.gBoxDimension.Name = "gBoxDimension";
            this.gBoxDimension.TabStop = false;
            // 
            // btnLimitExceed
            // 
            this.btnLimitExceed.BackColor = System.Drawing.Color.Yellow;
            resources.ApplyResources(this.btnLimitExceed, "btnLimitExceed");
            this.btnLimitExceed.Name = "btnLimitExceed";
            this.btnLimitExceed.UseVisualStyleBackColor = false;
            this.btnLimitExceed.Click += new System.EventHandler(this.btnLimitExceed_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnOffsetApply);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.tbOffsetY);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.tbOffsetX);
            this.groupBox4.Controls.Add(this.rBOrigin9);
            this.groupBox4.Controls.Add(this.rBOrigin8);
            this.groupBox4.Controls.Add(this.rBOrigin7);
            this.groupBox4.Controls.Add(this.rBOrigin6);
            this.groupBox4.Controls.Add(this.rBOrigin5);
            this.groupBox4.Controls.Add(this.rBOrigin4);
            this.groupBox4.Controls.Add(this.rBOrigin3);
            this.groupBox4.Controls.Add(this.rBOrigin2);
            this.groupBox4.Controls.Add(this.rBOrigin1);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // btnOffsetApply
            // 
            resources.ApplyResources(this.btnOffsetApply, "btnOffsetApply");
            this.btnOffsetApply.Name = "btnOffsetApply";
            this.btnOffsetApply.UseVisualStyleBackColor = true;
            this.btnOffsetApply.Click += new System.EventHandler(this.btnOffsetApply_Click);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // tbOffsetY
            // 
            resources.ApplyResources(this.tbOffsetY, "tbOffsetY");
            this.tbOffsetY.Name = "tbOffsetY";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // tbOffsetX
            // 
            resources.ApplyResources(this.tbOffsetX, "tbOffsetX");
            this.tbOffsetX.Name = "tbOffsetX";
            // 
            // rBOrigin9
            // 
            resources.ApplyResources(this.rBOrigin9, "rBOrigin9");
            this.rBOrigin9.Name = "rBOrigin9";
            this.rBOrigin9.UseVisualStyleBackColor = true;
            // 
            // rBOrigin8
            // 
            resources.ApplyResources(this.rBOrigin8, "rBOrigin8");
            this.rBOrigin8.Name = "rBOrigin8";
            this.rBOrigin8.UseVisualStyleBackColor = true;
            // 
            // rBOrigin7
            // 
            resources.ApplyResources(this.rBOrigin7, "rBOrigin7");
            this.rBOrigin7.Name = "rBOrigin7";
            this.rBOrigin7.UseVisualStyleBackColor = true;
            // 
            // rBOrigin6
            // 
            resources.ApplyResources(this.rBOrigin6, "rBOrigin6");
            this.rBOrigin6.Name = "rBOrigin6";
            this.rBOrigin6.UseVisualStyleBackColor = true;
            // 
            // rBOrigin5
            // 
            resources.ApplyResources(this.rBOrigin5, "rBOrigin5");
            this.rBOrigin5.Checked = true;
            this.rBOrigin5.Name = "rBOrigin5";
            this.rBOrigin5.TabStop = true;
            this.rBOrigin5.UseVisualStyleBackColor = true;
            // 
            // rBOrigin4
            // 
            resources.ApplyResources(this.rBOrigin4, "rBOrigin4");
            this.rBOrigin4.Name = "rBOrigin4";
            this.rBOrigin4.UseVisualStyleBackColor = true;
            // 
            // rBOrigin3
            // 
            resources.ApplyResources(this.rBOrigin3, "rBOrigin3");
            this.rBOrigin3.Name = "rBOrigin3";
            this.rBOrigin3.UseVisualStyleBackColor = true;
            // 
            // rBOrigin2
            // 
            resources.ApplyResources(this.rBOrigin2, "rBOrigin2");
            this.rBOrigin2.Name = "rBOrigin2";
            this.rBOrigin2.UseVisualStyleBackColor = true;
            // 
            // rBOrigin1
            // 
            resources.ApplyResources(this.rBOrigin1, "rBOrigin1");
            this.rBOrigin1.Name = "rBOrigin1";
            this.rBOrigin1.UseVisualStyleBackColor = true;
            // 
            // lbDimension
            // 
            this.lbDimension.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            resources.ApplyResources(this.lbDimension, "lbDimension");
            this.lbDimension.HideSelection = false;
            this.lbDimension.Name = "lbDimension";
            this.lbDimension.ReadOnly = true;
            // 
            // gBoxOverride
            // 
            this.gBoxOverride.Controls.Add(this.gBOverrideRGB);
            this.gBoxOverride.Controls.Add(this.gBOverrideASGB);
            this.gBoxOverride.Controls.Add(this.gBOverrideFRGB);
            this.gBoxOverride.Controls.Add(this.gBOverrideSSGB);
            resources.ApplyResources(this.gBoxOverride, "gBoxOverride");
            this.gBoxOverride.Name = "gBoxOverride";
            this.gBoxOverride.TabStop = false;
            // 
            // gBOverrideRGB
            // 
            this.gBOverrideRGB.Controls.Add(this.label12);
            this.gBOverrideRGB.Controls.Add(this.btnOverrideRapid0);
            this.gBOverrideRGB.Controls.Add(this.label13);
            this.gBOverrideRGB.Controls.Add(this.btnOverrideRapid2);
            this.gBOverrideRGB.Controls.Add(this.lblOverrideRapidValue);
            this.gBOverrideRGB.Controls.Add(this.btnOverrideRapid1);
            resources.ApplyResources(this.gBOverrideRGB, "gBOverrideRGB");
            this.gBOverrideRGB.Name = "gBOverrideRGB";
            this.gBOverrideRGB.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // btnOverrideRapid0
            // 
            resources.ApplyResources(this.btnOverrideRapid0, "btnOverrideRapid0");
            this.btnOverrideRapid0.Name = "btnOverrideRapid0";
            this.btnOverrideRapid0.UseVisualStyleBackColor = true;
            this.btnOverrideRapid0.Click += new System.EventHandler(this.btnOverrideRapid0_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // btnOverrideRapid2
            // 
            resources.ApplyResources(this.btnOverrideRapid2, "btnOverrideRapid2");
            this.btnOverrideRapid2.Name = "btnOverrideRapid2";
            this.btnOverrideRapid2.UseVisualStyleBackColor = true;
            this.btnOverrideRapid2.Click += new System.EventHandler(this.btnOverrideRapid2_Click);
            // 
            // lblOverrideRapidValue
            // 
            resources.ApplyResources(this.lblOverrideRapidValue, "lblOverrideRapidValue");
            this.lblOverrideRapidValue.Name = "lblOverrideRapidValue";
            // 
            // btnOverrideRapid1
            // 
            resources.ApplyResources(this.btnOverrideRapid1, "btnOverrideRapid1");
            this.btnOverrideRapid1.Name = "btnOverrideRapid1";
            this.btnOverrideRapid1.UseVisualStyleBackColor = true;
            this.btnOverrideRapid1.Click += new System.EventHandler(this.btnOverrideRapid1_Click);
            // 
            // gBOverrideASGB
            // 
            this.gBOverrideASGB.Controls.Add(this.btnOverrideSpindle);
            this.gBOverrideASGB.Controls.Add(this.btnOverrideMist);
            this.gBOverrideASGB.Controls.Add(this.btnOverrideFlood);
            resources.ApplyResources(this.gBOverrideASGB, "gBOverrideASGB");
            this.gBOverrideASGB.Name = "gBOverrideASGB";
            this.gBOverrideASGB.TabStop = false;
            // 
            // btnOverrideSpindle
            // 
            resources.ApplyResources(this.btnOverrideSpindle, "btnOverrideSpindle");
            this.btnOverrideSpindle.Image = global::GRBL_Plotter.Properties.Resources.led_off;
            this.btnOverrideSpindle.Name = "btnOverrideSpindle";
            this.toolTip1.SetToolTip(this.btnOverrideSpindle, resources.GetString("btnOverrideSpindle.ToolTip"));
            this.btnOverrideSpindle.UseVisualStyleBackColor = true;
            this.btnOverrideSpindle.Click += new System.EventHandler(this.btnOverrideSpindle_Click);
            // 
            // btnOverrideMist
            // 
            resources.ApplyResources(this.btnOverrideMist, "btnOverrideMist");
            this.btnOverrideMist.Image = global::GRBL_Plotter.Properties.Resources.led_off;
            this.btnOverrideMist.Name = "btnOverrideMist";
            this.toolTip1.SetToolTip(this.btnOverrideMist, resources.GetString("btnOverrideMist.ToolTip"));
            this.btnOverrideMist.UseVisualStyleBackColor = true;
            this.btnOverrideMist.Click += new System.EventHandler(this.btnOverrideMist_Click);
            // 
            // btnOverrideFlood
            // 
            resources.ApplyResources(this.btnOverrideFlood, "btnOverrideFlood");
            this.btnOverrideFlood.Image = global::GRBL_Plotter.Properties.Resources.led_off;
            this.btnOverrideFlood.Name = "btnOverrideFlood";
            this.btnOverrideFlood.UseVisualStyleBackColor = true;
            this.btnOverrideFlood.Click += new System.EventHandler(this.btnOverrideFlood_Click);
            // 
            // gBOverrideFRGB
            // 
            this.gBOverrideFRGB.Controls.Add(this.lblStatusFeed);
            this.gBOverrideFRGB.Controls.Add(this.label5);
            this.gBOverrideFRGB.Controls.Add(this.label7);
            this.gBOverrideFRGB.Controls.Add(this.lblOverrideFRValue);
            this.gBOverrideFRGB.Controls.Add(this.btnOverrideFR1);
            this.gBOverrideFRGB.Controls.Add(this.btnOverrideFR2);
            this.gBOverrideFRGB.Controls.Add(this.btnOverrideFR0);
            this.gBOverrideFRGB.Controls.Add(this.btnOverrideFR4);
            this.gBOverrideFRGB.Controls.Add(this.btnOverrideFR3);
            resources.ApplyResources(this.gBOverrideFRGB, "gBOverrideFRGB");
            this.gBOverrideFRGB.Name = "gBOverrideFRGB";
            this.gBOverrideFRGB.TabStop = false;
            // 
            // lblStatusFeed
            // 
            resources.ApplyResources(this.lblStatusFeed, "lblStatusFeed");
            this.lblStatusFeed.Name = "lblStatusFeed";
            this.toolTip1.SetToolTip(this.lblStatusFeed, resources.GetString("lblStatusFeed.ToolTip"));
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // lblOverrideFRValue
            // 
            resources.ApplyResources(this.lblOverrideFRValue, "lblOverrideFRValue");
            this.lblOverrideFRValue.Name = "lblOverrideFRValue";
            // 
            // btnOverrideFR1
            // 
            resources.ApplyResources(this.btnOverrideFR1, "btnOverrideFR1");
            this.btnOverrideFR1.Name = "btnOverrideFR1";
            this.btnOverrideFR1.UseVisualStyleBackColor = true;
            this.btnOverrideFR1.Click += new System.EventHandler(this.btnOverrideFR1_Click);
            // 
            // btnOverrideFR2
            // 
            resources.ApplyResources(this.btnOverrideFR2, "btnOverrideFR2");
            this.btnOverrideFR2.Name = "btnOverrideFR2";
            this.btnOverrideFR2.UseVisualStyleBackColor = true;
            this.btnOverrideFR2.Click += new System.EventHandler(this.btnOverrideFR2_Click);
            // 
            // btnOverrideFR0
            // 
            resources.ApplyResources(this.btnOverrideFR0, "btnOverrideFR0");
            this.btnOverrideFR0.Name = "btnOverrideFR0";
            this.btnOverrideFR0.UseVisualStyleBackColor = true;
            this.btnOverrideFR0.Click += new System.EventHandler(this.btnOverrideFR0_Click);
            // 
            // btnOverrideFR4
            // 
            resources.ApplyResources(this.btnOverrideFR4, "btnOverrideFR4");
            this.btnOverrideFR4.Name = "btnOverrideFR4";
            this.btnOverrideFR4.UseVisualStyleBackColor = true;
            this.btnOverrideFR4.Click += new System.EventHandler(this.btnOverrideFR4_Click);
            // 
            // btnOverrideFR3
            // 
            resources.ApplyResources(this.btnOverrideFR3, "btnOverrideFR3");
            this.btnOverrideFR3.Name = "btnOverrideFR3";
            this.btnOverrideFR3.UseVisualStyleBackColor = true;
            this.btnOverrideFR3.Click += new System.EventHandler(this.btnOverrideFR3_Click);
            // 
            // gBOverrideSSGB
            // 
            this.gBOverrideSSGB.Controls.Add(this.lblStatusSpeed);
            this.gBOverrideSSGB.Controls.Add(this.label8);
            this.gBOverrideSSGB.Controls.Add(this.label10);
            this.gBOverrideSSGB.Controls.Add(this.lblOverrideSSValue);
            this.gBOverrideSSGB.Controls.Add(this.btnOverrideSS2);
            this.gBOverrideSSGB.Controls.Add(this.btnOverrideSS0);
            this.gBOverrideSSGB.Controls.Add(this.btnOverrideSS1);
            this.gBOverrideSSGB.Controls.Add(this.btnOverrideSS4);
            this.gBOverrideSSGB.Controls.Add(this.btnOverrideSS3);
            resources.ApplyResources(this.gBOverrideSSGB, "gBOverrideSSGB");
            this.gBOverrideSSGB.Name = "gBOverrideSSGB";
            this.gBOverrideSSGB.TabStop = false;
            // 
            // lblStatusSpeed
            // 
            resources.ApplyResources(this.lblStatusSpeed, "lblStatusSpeed");
            this.lblStatusSpeed.Name = "lblStatusSpeed";
            this.toolTip1.SetToolTip(this.lblStatusSpeed, resources.GetString("lblStatusSpeed.ToolTip"));
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // lblOverrideSSValue
            // 
            resources.ApplyResources(this.lblOverrideSSValue, "lblOverrideSSValue");
            this.lblOverrideSSValue.Name = "lblOverrideSSValue";
            // 
            // btnOverrideSS2
            // 
            resources.ApplyResources(this.btnOverrideSS2, "btnOverrideSS2");
            this.btnOverrideSS2.Name = "btnOverrideSS2";
            this.btnOverrideSS2.UseVisualStyleBackColor = true;
            this.btnOverrideSS2.Click += new System.EventHandler(this.btnOverrideSS2_Click);
            // 
            // btnOverrideSS0
            // 
            resources.ApplyResources(this.btnOverrideSS0, "btnOverrideSS0");
            this.btnOverrideSS0.Name = "btnOverrideSS0";
            this.btnOverrideSS0.UseVisualStyleBackColor = true;
            this.btnOverrideSS0.Click += new System.EventHandler(this.btnOverrideSS0_Click);
            // 
            // btnOverrideSS1
            // 
            resources.ApplyResources(this.btnOverrideSS1, "btnOverrideSS1");
            this.btnOverrideSS1.Name = "btnOverrideSS1";
            this.btnOverrideSS1.UseVisualStyleBackColor = true;
            this.btnOverrideSS1.Click += new System.EventHandler(this.btnOverrideSS1_Click);
            // 
            // btnOverrideSS4
            // 
            resources.ApplyResources(this.btnOverrideSS4, "btnOverrideSS4");
            this.btnOverrideSS4.Name = "btnOverrideSS4";
            this.btnOverrideSS4.UseVisualStyleBackColor = true;
            this.btnOverrideSS4.Click += new System.EventHandler(this.btnOverrideSS4_Click);
            // 
            // btnOverrideSS3
            // 
            resources.ApplyResources(this.btnOverrideSS3, "btnOverrideSS3");
            this.btnOverrideSS3.Name = "btnOverrideSS3";
            this.btnOverrideSS3.UseVisualStyleBackColor = true;
            this.btnOverrideSS3.Click += new System.EventHandler(this.btnOverrideSS3_Click);
            // 
            // tLPRechts
            // 
            resources.ApplyResources(this.tLPRechts, "tLPRechts");
            this.tLPRechts.Controls.Add(this.tLPRechtsUnten, 0, 1);
            this.tLPRechts.Controls.Add(this.tLPRechtsOben, 0, 0);
            this.tLPRechts.Name = "tLPRechts";
            // 
            // tLPRechtsUnten
            // 
            resources.ApplyResources(this.tLPRechtsUnten, "tLPRechtsUnten");
            this.tLPRechtsUnten.Controls.Add(this.tLPRechtsUntenRechts, 1, 0);
            this.tLPRechtsUnten.Controls.Add(this.tLPMitteUnten, 0, 0);
            this.tLPRechtsUnten.Name = "tLPRechtsUnten";
            // 
            // tLPRechtsUntenRechts
            // 
            resources.ApplyResources(this.tLPRechtsUntenRechts, "tLPRechtsUntenRechts");
            this.tLPRechtsUntenRechts.Controls.Add(this.gB_Jogging, 0, 0);
            this.tLPRechtsUntenRechts.Controls.Add(this.tLPRechtsUntenRechtsMitte, 0, 1);
            this.tLPRechtsUntenRechts.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tLPRechtsUntenRechts.Name = "tLPRechtsUntenRechts";
            // 
            // gB_Jogging
            // 
            resources.ApplyResources(this.gB_Jogging, "gB_Jogging");
            this.gB_Jogging.Controls.Add(this.cBServoButtons);
            this.gB_Jogging.Controls.Add(this.btnPenDown);
            this.gB_Jogging.Controls.Add(this.btnPenUp);
            this.gB_Jogging.Controls.Add(this.btnJogStop);
            this.gB_Jogging.Controls.Add(this.gB_Jog0);
            this.gB_Jogging.Controls.Add(this.lblTool);
            this.gB_Jogging.Controls.Add(this.cBTool);
            this.gB_Jogging.Controls.Add(this.lblSpeed);
            this.gB_Jogging.Controls.Add(this.tBSpeed);
            this.gB_Jogging.Controls.Add(this.cBCoolant);
            this.gB_Jogging.Controls.Add(this.cBSpindle);
            this.gB_Jogging.Name = "gB_Jogging";
            this.gB_Jogging.TabStop = false;
            this.gB_Jogging.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.gB_Jogging.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // cBServoButtons
            // 
            resources.ApplyResources(this.cBServoButtons, "cBServoButtons");
            this.cBServoButtons.Checked = global::GRBL_Plotter.Properties.Settings.Default.guiPenUpDownButtonsShow;
            this.cBServoButtons.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "guiPenUpDownButtonsShow", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.cBServoButtons.Name = "cBServoButtons";
            this.cBServoButtons.UseVisualStyleBackColor = true;
            this.cBServoButtons.CheckedChanged += new System.EventHandler(this.cBServoButtons_CheckedChanged);
            // 
            // btnPenDown
            // 
            resources.ApplyResources(this.btnPenDown, "btnPenDown");
            this.btnPenDown.Name = "btnPenDown";
            this.toolTip1.SetToolTip(this.btnPenDown, resources.GetString("btnPenDown.ToolTip"));
            this.btnPenDown.UseVisualStyleBackColor = true;
            this.btnPenDown.Click += new System.EventHandler(this.btnPenDown_Click);
            // 
            // btnPenUp
            // 
            resources.ApplyResources(this.btnPenUp, "btnPenUp");
            this.btnPenUp.Name = "btnPenUp";
            this.toolTip1.SetToolTip(this.btnPenUp, resources.GetString("btnPenUp.ToolTip"));
            this.btnPenUp.UseVisualStyleBackColor = true;
            this.btnPenUp.Click += new System.EventHandler(this.btnPenUp_Click);
            // 
            // btnJogStop
            // 
            this.btnJogStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            resources.ApplyResources(this.btnJogStop, "btnJogStop");
            this.btnJogStop.Name = "btnJogStop";
            this.toolTip1.SetToolTip(this.btnJogStop, resources.GetString("btnJogStop.ToolTip"));
            this.btnJogStop.UseVisualStyleBackColor = false;
            this.btnJogStop.Click += new System.EventHandler(this.btnJogStop_Click);
            // 
            // gB_Jog0
            // 
            this.gB_Jog0.Controls.Add(this.btnJogZeroA);
            this.gB_Jog0.Controls.Add(this.btnJogZeroX);
            this.gB_Jog0.Controls.Add(this.btnJogZeroXY);
            this.gB_Jog0.Controls.Add(this.btnJogZeroY);
            this.gB_Jog0.Controls.Add(this.btnJogZeroZ);
            this.gB_Jog0.Controls.Add(this.cBMoveG0);
            resources.ApplyResources(this.gB_Jog0, "gB_Jog0");
            this.gB_Jog0.Name = "gB_Jog0";
            this.gB_Jog0.TabStop = false;
            // 
            // btnJogZeroA
            // 
            resources.ApplyResources(this.btnJogZeroA, "btnJogZeroA");
            this.btnJogZeroA.Name = "btnJogZeroA";
            this.toolTip1.SetToolTip(this.btnJogZeroA, resources.GetString("btnJogZeroA.ToolTip"));
            this.btnJogZeroA.UseVisualStyleBackColor = true;
            this.btnJogZeroA.Click += new System.EventHandler(this.btnJogZeroA_Click);
            // 
            // btnJogZeroX
            // 
            resources.ApplyResources(this.btnJogZeroX, "btnJogZeroX");
            this.btnJogZeroX.Name = "btnJogZeroX";
            this.toolTip1.SetToolTip(this.btnJogZeroX, resources.GetString("btnJogZeroX.ToolTip"));
            this.btnJogZeroX.UseVisualStyleBackColor = true;
            this.btnJogZeroX.Click += new System.EventHandler(this.btnJogX_Click);
            // 
            // btnJogZeroXY
            // 
            resources.ApplyResources(this.btnJogZeroXY, "btnJogZeroXY");
            this.btnJogZeroXY.Name = "btnJogZeroXY";
            this.toolTip1.SetToolTip(this.btnJogZeroXY, resources.GetString("btnJogZeroXY.ToolTip"));
            this.btnJogZeroXY.UseVisualStyleBackColor = true;
            this.btnJogZeroXY.Click += new System.EventHandler(this.btnJogXY_Click);
            // 
            // btnJogZeroY
            // 
            resources.ApplyResources(this.btnJogZeroY, "btnJogZeroY");
            this.btnJogZeroY.Name = "btnJogZeroY";
            this.toolTip1.SetToolTip(this.btnJogZeroY, resources.GetString("btnJogZeroY.ToolTip"));
            this.btnJogZeroY.UseVisualStyleBackColor = true;
            this.btnJogZeroY.Click += new System.EventHandler(this.btnJogY_Click);
            // 
            // btnJogZeroZ
            // 
            resources.ApplyResources(this.btnJogZeroZ, "btnJogZeroZ");
            this.btnJogZeroZ.Name = "btnJogZeroZ";
            this.toolTip1.SetToolTip(this.btnJogZeroZ, resources.GetString("btnJogZeroZ.ToolTip"));
            this.btnJogZeroZ.UseVisualStyleBackColor = true;
            this.btnJogZeroZ.Click += new System.EventHandler(this.btnJogZ_Click);
            // 
            // cBMoveG0
            // 
            resources.ApplyResources(this.cBMoveG0, "cBMoveG0");
            this.cBMoveG0.Name = "cBMoveG0";
            this.cBMoveG0.UseVisualStyleBackColor = true;
            // 
            // lblTool
            // 
            resources.ApplyResources(this.lblTool, "lblTool");
            this.lblTool.Name = "lblTool";
            // 
            // cBTool
            // 
            resources.ApplyResources(this.cBTool, "cBTool");
            this.cBTool.Name = "cBTool";
            this.cBTool.UseVisualStyleBackColor = true;
            this.cBTool.CheckedChanged += new System.EventHandler(this.cBTool_CheckedChanged);
            // 
            // lblSpeed
            // 
            resources.ApplyResources(this.lblSpeed, "lblSpeed");
            this.lblSpeed.Name = "lblSpeed";
            // 
            // tBSpeed
            // 
            this.tBSpeed.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::GRBL_Plotter.Properties.Settings.Default, "guiSpindleSpeed", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.tBSpeed, "tBSpeed");
            this.tBSpeed.Name = "tBSpeed";
            this.tBSpeed.Text = global::GRBL_Plotter.Properties.Settings.Default.guiSpindleSpeed;
            // 
            // cBCoolant
            // 
            resources.ApplyResources(this.cBCoolant, "cBCoolant");
            this.cBCoolant.Name = "cBCoolant";
            this.cBCoolant.UseVisualStyleBackColor = true;
            this.cBCoolant.CheckedChanged += new System.EventHandler(this.cBCoolant_CheckedChanged);
            // 
            // cBSpindle
            // 
            resources.ApplyResources(this.cBSpindle, "cBSpindle");
            this.cBSpindle.Name = "cBSpindle";
            this.cBSpindle.UseVisualStyleBackColor = true;
            this.cBSpindle.CheckedChanged += new System.EventHandler(this.cBSpindle_CheckedChanged);
            // 
            // tLPRechtsUntenRechtsMitte
            // 
            resources.ApplyResources(this.tLPRechtsUntenRechtsMitte, "tLPRechtsUntenRechtsMitte");
            this.tLPRechtsUntenRechtsMitte.Controls.Add(this.virtualJoystickA, 2, 0);
            this.tLPRechtsUntenRechtsMitte.Controls.Add(this.virtualJoystickXY, 0, 0);
            this.tLPRechtsUntenRechtsMitte.Controls.Add(this.virtualJoystickZ, 1, 0);
            this.tLPRechtsUntenRechtsMitte.Controls.Add(this.virtualJoystickB, 3, 0);
            this.tLPRechtsUntenRechtsMitte.Controls.Add(this.virtualJoystickC, 4, 0);
            this.tLPRechtsUntenRechtsMitte.Name = "tLPRechtsUntenRechtsMitte";
            this.tLPRechtsUntenRechtsMitte.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.tLPRechtsUntenRechtsMitte.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            // 
            // virtualJoystickA
            // 
            resources.ApplyResources(this.virtualJoystickA, "virtualJoystickA");
            this.virtualJoystickA.Joystick2Dimension = false;
            this.virtualJoystickA.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickA.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickA.JoystickRaster = 5;
            this.virtualJoystickA.JoystickRasterMark = 0;
            this.virtualJoystickA.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickA.JoystickText = "A";
            this.virtualJoystickA.Name = "virtualJoystickA";
            this.virtualJoystickA.TabStop = false;
            this.toolTip1.SetToolTip(this.virtualJoystickA, resources.GetString("virtualJoystickA.ToolTip"));
            this.virtualJoystickA.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickA_JoyStickEvent);
            this.virtualJoystickA.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickA.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            this.virtualJoystickA.MouseUp += new System.Windows.Forms.MouseEventHandler(this.virtualJoystickXY_MouseUp);
            this.virtualJoystickA.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.virtualJoystickXY_PreviewKeyDown);
            // 
            // virtualJoystickXY
            // 
            resources.ApplyResources(this.virtualJoystickXY, "virtualJoystickXY");
            this.virtualJoystickXY.Joystick2Dimension = true;
            this.virtualJoystickXY.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickXY.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickXY.JoystickRaster = 5;
            this.virtualJoystickXY.JoystickRasterMark = 0;
            this.virtualJoystickXY.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickXY.JoystickText = "X / Y";
            this.virtualJoystickXY.Name = "virtualJoystickXY";
            this.virtualJoystickXY.TabStop = false;
            this.toolTip1.SetToolTip(this.virtualJoystickXY, resources.GetString("virtualJoystickXY.ToolTip"));
            this.virtualJoystickXY.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickXY_JoyStickEvent);
            this.virtualJoystickXY.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickXY.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            this.virtualJoystickXY.MouseUp += new System.Windows.Forms.MouseEventHandler(this.virtualJoystickXY_MouseUp);
            this.virtualJoystickXY.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.virtualJoystickXY_PreviewKeyDown);
            // 
            // virtualJoystickZ
            // 
            resources.ApplyResources(this.virtualJoystickZ, "virtualJoystickZ");
            this.virtualJoystickZ.Joystick2Dimension = false;
            this.virtualJoystickZ.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickZ.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickZ.JoystickRaster = 5;
            this.virtualJoystickZ.JoystickRasterMark = 0;
            this.virtualJoystickZ.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickZ.JoystickText = "Z";
            this.virtualJoystickZ.Name = "virtualJoystickZ";
            this.virtualJoystickZ.TabStop = false;
            this.toolTip1.SetToolTip(this.virtualJoystickZ, resources.GetString("virtualJoystickZ.ToolTip"));
            this.virtualJoystickZ.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickZ_JoyStickEvent);
            this.virtualJoystickZ.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickZ.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            this.virtualJoystickZ.MouseUp += new System.Windows.Forms.MouseEventHandler(this.virtualJoystickXY_MouseUp);
            this.virtualJoystickZ.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.virtualJoystickXY_PreviewKeyDown);
            // 
            // virtualJoystickB
            // 
            resources.ApplyResources(this.virtualJoystickB, "virtualJoystickB");
            this.virtualJoystickB.Joystick2Dimension = false;
            this.virtualJoystickB.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickB.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickB.JoystickRaster = 5;
            this.virtualJoystickB.JoystickRasterMark = 0;
            this.virtualJoystickB.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickB.JoystickText = "B";
            this.virtualJoystickB.Name = "virtualJoystickB";
            this.virtualJoystickB.TabStop = false;
            this.toolTip1.SetToolTip(this.virtualJoystickB, resources.GetString("virtualJoystickB.ToolTip"));
            this.virtualJoystickB.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickB_JoyStickEvent);
            this.virtualJoystickB.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickB.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            this.virtualJoystickB.MouseUp += new System.Windows.Forms.MouseEventHandler(this.virtualJoystickXY_MouseUp);
            this.virtualJoystickB.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.virtualJoystickXY_PreviewKeyDown);
            // 
            // virtualJoystickC
            // 
            resources.ApplyResources(this.virtualJoystickC, "virtualJoystickC");
            this.virtualJoystickC.Joystick2Dimension = false;
            this.virtualJoystickC.JoystickActive = System.Drawing.Color.Red;
            this.virtualJoystickC.JoystickLabel = new double[] {
        0.1D,
        0.5D,
        1D,
        5D,
        10D,
        50D};
            this.virtualJoystickC.JoystickRaster = 5;
            this.virtualJoystickC.JoystickRasterMark = 0;
            this.virtualJoystickC.JoystickStanby = System.Drawing.Color.Orange;
            this.virtualJoystickC.JoystickText = "C";
            this.virtualJoystickC.Name = "virtualJoystickC";
            this.virtualJoystickC.TabStop = false;
            this.toolTip1.SetToolTip(this.virtualJoystickC, resources.GetString("virtualJoystickC.ToolTip"));
            this.virtualJoystickC.JoyStickEvent += new virtualJoystick.JogEventHandler(this.virtualJoystickC_JoyStickEvent);
            this.virtualJoystickC.Enter += new System.EventHandler(this.virtualJoystickXY_Enter);
            this.virtualJoystickC.Leave += new System.EventHandler(this.virtualJoystickXY_Leave);
            this.virtualJoystickC.MouseUp += new System.Windows.Forms.MouseEventHandler(this.virtualJoystickXY_MouseUp);
            this.virtualJoystickC.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.virtualJoystickXY_PreviewKeyDown);
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.cBSendJogStop, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // cBSendJogStop
            // 
            this.cBSendJogStop.Checked = global::GRBL_Plotter.Properties.Settings.Default.ctrlSendStopJog;
            this.cBSendJogStop.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cBSendJogStop.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::GRBL_Plotter.Properties.Settings.Default, "ctrlSendStopJog", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            resources.ApplyResources(this.cBSendJogStop, "cBSendJogStop");
            this.cBSendJogStop.Name = "cBSendJogStop";
            this.cBSendJogStop.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 1, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // tableLayoutPanel4
            // 
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Controls.Add(this.btnOverrideDoor, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnResume, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.btnFeedHold, 0, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            // 
            // btnOverrideDoor
            // 
            resources.ApplyResources(this.btnOverrideDoor, "btnOverrideDoor");
            this.btnOverrideDoor.Name = "btnOverrideDoor";
            this.toolTip1.SetToolTip(this.btnOverrideDoor, resources.GetString("btnOverrideDoor.ToolTip"));
            this.btnOverrideDoor.UseVisualStyleBackColor = true;
            this.btnOverrideDoor.Click += new System.EventHandler(this.btnOverrideDoor_Click);
            // 
            // btnResume
            // 
            resources.ApplyResources(this.btnResume, "btnResume");
            this.btnResume.Name = "btnResume";
            this.toolTip1.SetToolTip(this.btnResume, resources.GetString("btnResume.ToolTip"));
            this.btnResume.UseVisualStyleBackColor = true;
            this.btnResume.Click += new System.EventHandler(this.btnResume_Click);
            // 
            // btnFeedHold
            // 
            resources.ApplyResources(this.btnFeedHold, "btnFeedHold");
            this.btnFeedHold.Name = "btnFeedHold";
            this.toolTip1.SetToolTip(this.btnFeedHold, resources.GetString("btnFeedHold.ToolTip"));
            this.btnFeedHold.UseVisualStyleBackColor = true;
            this.btnFeedHold.Click += new System.EventHandler(this.btnFeedHold_Click);
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.Controls.Add(this.btnKillAlarm, 0, 1);
            this.tableLayoutPanel5.Controls.Add(this.btnReset, 0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            // 
            // btnKillAlarm
            // 
            resources.ApplyResources(this.btnKillAlarm, "btnKillAlarm");
            this.btnKillAlarm.Name = "btnKillAlarm";
            this.toolTip1.SetToolTip(this.btnKillAlarm, resources.GetString("btnKillAlarm.ToolTip"));
            this.btnKillAlarm.UseVisualStyleBackColor = true;
            this.btnKillAlarm.Click += new System.EventHandler(this.btnKillAlarm_Click);
            // 
            // btnReset
            // 
            resources.ApplyResources(this.btnReset, "btnReset");
            this.btnReset.Name = "btnReset";
            this.toolTip1.SetToolTip(this.btnReset, resources.GetString("btnReset.ToolTip"));
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tLPMitteUnten
            // 
            resources.ApplyResources(this.tLPMitteUnten, "tLPMitteUnten");
            this.tLPMitteUnten.Controls.Add(this.pictureBox1, 0, 0);
            this.tLPMitteUnten.Controls.Add(this.tBURL, 0, 1);
            this.tLPMitteUnten.Name = "tLPMitteUnten";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::GRBL_Plotter.Properties.Resources.modell;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.pictureBox1_SizeChanged);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseHover += new System.EventHandler(this.pictureBox1_MouseHover);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseWheel);
            // 
            // cmsPictureBox
            // 
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unDo2ToolStripMenuItem,
            this.toolStripSeparator17,
            this.cmsPicBoxMoveToMarkedPosition,
            this.cmsPicBoxZeroXYAtMarkedPosition,
            this.cmsPicBoxMoveGraphicsOrigin,
            this.toolStripSeparator9,
            this.cmsPicBoxResetZooming,
            this.toolStripSeparator8,
            this.cmsPicBoxPasteFromClipboard,
            this.cmsPicBoxReloadFile,
            this.deletenotMarkToolStripMenuItem,
            this.toolStripSeparator1,
            this.cmsPicBoxMarkFirstPos,
            this.cmsPicBoxDeletePath,
            this.cmsPicBoxCropSelectedPath,
            this.cmsPicBoxMoveSelectedPathInCode,
            this.cmsPicBoxReverseSelectedPath,
            this.toolStripSeparator10,
            this.cmsPicBoxSetGCodeAsBackground,
            this.cmsPicBoxClearBackground,
            this.copyContentTroClipboardToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            resources.ApplyResources(this.cmsPictureBox, "cmsPictureBox");
            // 
            // unDo2ToolStripMenuItem
            // 
            resources.ApplyResources(this.unDo2ToolStripMenuItem, "unDo2ToolStripMenuItem");
            this.unDo2ToolStripMenuItem.Name = "unDo2ToolStripMenuItem";
            this.unDo2ToolStripMenuItem.Click += new System.EventHandler(this.unDoToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // cmsPicBoxMoveToMarkedPosition
            // 
            this.cmsPicBoxMoveToMarkedPosition.Name = "cmsPicBoxMoveToMarkedPosition";
            resources.ApplyResources(this.cmsPicBoxMoveToMarkedPosition, "cmsPicBoxMoveToMarkedPosition");
            this.cmsPicBoxMoveToMarkedPosition.Click += new System.EventHandler(this.cmsPicBoxMoveToMarkedPosition_Click);
            // 
            // cmsPicBoxZeroXYAtMarkedPosition
            // 
            this.cmsPicBoxZeroXYAtMarkedPosition.Name = "cmsPicBoxZeroXYAtMarkedPosition";
            resources.ApplyResources(this.cmsPicBoxZeroXYAtMarkedPosition, "cmsPicBoxZeroXYAtMarkedPosition");
            this.cmsPicBoxZeroXYAtMarkedPosition.Click += new System.EventHandler(this.cmsPicBoxZeroXYAtMarkedPosition_Click);
            // 
            // cmsPicBoxMoveGraphicsOrigin
            // 
            this.cmsPicBoxMoveGraphicsOrigin.Name = "cmsPicBoxMoveGraphicsOrigin";
            resources.ApplyResources(this.cmsPicBoxMoveGraphicsOrigin, "cmsPicBoxMoveGraphicsOrigin");
            this.cmsPicBoxMoveGraphicsOrigin.Click += new System.EventHandler(this.cmsPicBoxMoveGraphicsOrigin_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // cmsPicBoxResetZooming
            // 
            this.cmsPicBoxResetZooming.Name = "cmsPicBoxResetZooming";
            resources.ApplyResources(this.cmsPicBoxResetZooming, "cmsPicBoxResetZooming");
            this.cmsPicBoxResetZooming.Click += new System.EventHandler(this.cmsPicBoxResetZooming_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // cmsPicBoxPasteFromClipboard
            // 
            this.cmsPicBoxPasteFromClipboard.Name = "cmsPicBoxPasteFromClipboard";
            resources.ApplyResources(this.cmsPicBoxPasteFromClipboard, "cmsPicBoxPasteFromClipboard");
            this.cmsPicBoxPasteFromClipboard.Click += new System.EventHandler(this.cmsPicBoxPasteFromClipboard_Click);
            // 
            // cmsPicBoxReloadFile
            // 
            this.cmsPicBoxReloadFile.Name = "cmsPicBoxReloadFile";
            resources.ApplyResources(this.cmsPicBoxReloadFile, "cmsPicBoxReloadFile");
            this.cmsPicBoxReloadFile.Click += new System.EventHandler(this.cmsPicBoxReloadFile_Click);
            // 
            // deletenotMarkToolStripMenuItem
            // 
            resources.ApplyResources(this.deletenotMarkToolStripMenuItem, "deletenotMarkToolStripMenuItem");
            this.deletenotMarkToolStripMenuItem.Name = "deletenotMarkToolStripMenuItem";
            this.deletenotMarkToolStripMenuItem.Click += new System.EventHandler(this.deletenotMarkToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // cmsPicBoxMarkFirstPos
            // 
            this.cmsPicBoxMarkFirstPos.Name = "cmsPicBoxMarkFirstPos";
            resources.ApplyResources(this.cmsPicBoxMarkFirstPos, "cmsPicBoxMarkFirstPos");
            this.cmsPicBoxMarkFirstPos.Click += new System.EventHandler(this.cmsPicBoxMoveToFirstPos_Click);
            // 
            // cmsPicBoxDeletePath
            // 
            resources.ApplyResources(this.cmsPicBoxDeletePath, "cmsPicBoxDeletePath");
            this.cmsPicBoxDeletePath.Name = "cmsPicBoxDeletePath";
            this.cmsPicBoxDeletePath.Click += new System.EventHandler(this.cmsPicBoxDeletePath_Click);
            // 
            // cmsPicBoxCropSelectedPath
            // 
            resources.ApplyResources(this.cmsPicBoxCropSelectedPath, "cmsPicBoxCropSelectedPath");
            this.cmsPicBoxCropSelectedPath.Name = "cmsPicBoxCropSelectedPath";
            this.cmsPicBoxCropSelectedPath.Click += new System.EventHandler(this.cmsPicBoxCropSelectedPath_Click);
            // 
            // cmsPicBoxMoveSelectedPathInCode
            // 
            resources.ApplyResources(this.cmsPicBoxMoveSelectedPathInCode, "cmsPicBoxMoveSelectedPathInCode");
            this.cmsPicBoxMoveSelectedPathInCode.Name = "cmsPicBoxMoveSelectedPathInCode";
            this.cmsPicBoxMoveSelectedPathInCode.Click += new System.EventHandler(this.cmsPicBoxMoveSelectedPathInCode_Click);
            // 
            // cmsPicBoxReverseSelectedPath
            // 
            resources.ApplyResources(this.cmsPicBoxReverseSelectedPath, "cmsPicBoxReverseSelectedPath");
            this.cmsPicBoxReverseSelectedPath.Name = "cmsPicBoxReverseSelectedPath";
            this.cmsPicBoxReverseSelectedPath.Click += new System.EventHandler(this.cmsPicBoxReverseSelectedPath_Click);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // cmsPicBoxSetGCodeAsBackground
            // 
            this.cmsPicBoxSetGCodeAsBackground.Name = "cmsPicBoxSetGCodeAsBackground";
            resources.ApplyResources(this.cmsPicBoxSetGCodeAsBackground, "cmsPicBoxSetGCodeAsBackground");
            this.cmsPicBoxSetGCodeAsBackground.Click += new System.EventHandler(this.cmsPicBoxSetGCodeAsBackground_Click);
            // 
            // cmsPicBoxClearBackground
            // 
            this.cmsPicBoxClearBackground.Name = "cmsPicBoxClearBackground";
            resources.ApplyResources(this.cmsPicBoxClearBackground, "cmsPicBoxClearBackground");
            this.cmsPicBoxClearBackground.Click += new System.EventHandler(this.cmsPicBoxClearBackground_Click);
            // 
            // copyContentTroClipboardToolStripMenuItem
            // 
            this.copyContentTroClipboardToolStripMenuItem.Name = "copyContentTroClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyContentTroClipboardToolStripMenuItem, "copyContentTroClipboardToolStripMenuItem");
            this.copyContentTroClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyContentTroClipboardToolStripMenuItem_Click);
            // 
            // tBURL
            // 
            resources.ApplyResources(this.tBURL, "tBURL");
            this.tBURL.Name = "tBURL";
            this.toolTip1.SetToolTip(this.tBURL, resources.GetString("tBURL.ToolTip"));
            this.tBURL.TextChanged += new System.EventHandler(this.tBURL_TextChanged);
            // 
            // tLPRechtsOben
            // 
            resources.ApplyResources(this.tLPRechtsOben, "tLPRechtsOben");
            this.tLPRechtsOben.Controls.Add(this.groupBox5, 1, 0);
            this.tLPRechtsOben.Controls.Add(this.groupBoxCoordinates, 0, 0);
            this.tLPRechtsOben.Name = "tLPRechtsOben";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.tLPCustomButton1);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // tLPCustomButton1
            // 
            resources.ApplyResources(this.tLPCustomButton1, "tLPCustomButton1");
            this.tLPCustomButton1.Controls.Add(this.tLPCustomButton2, 0, 0);
            this.tLPCustomButton1.Controls.Add(this.flowLayoutPanel1, 1, 0);
            this.tLPCustomButton1.Name = "tLPCustomButton1";
            // 
            // tLPCustomButton2
            // 
            resources.ApplyResources(this.tLPCustomButton2, "tLPCustomButton2");
            this.tLPCustomButton2.Controls.Add(this.btnCustom1, 0, 0);
            this.tLPCustomButton2.Controls.Add(this.btnCustom2, 0, 1);
            this.tLPCustomButton2.Controls.Add(this.btnCustom3, 0, 2);
            this.tLPCustomButton2.Controls.Add(this.btnCustom4, 0, 3);
            this.tLPCustomButton2.Controls.Add(this.btnCustom5, 1, 0);
            this.tLPCustomButton2.Controls.Add(this.btnCustom6, 1, 1);
            this.tLPCustomButton2.Controls.Add(this.btnCustom7, 1, 2);
            this.tLPCustomButton2.Controls.Add(this.btnCustom8, 1, 3);
            this.tLPCustomButton2.Controls.Add(this.btnCustom9, 2, 0);
            this.tLPCustomButton2.Controls.Add(this.btnCustom10, 2, 1);
            this.tLPCustomButton2.Controls.Add(this.btnCustom11, 2, 2);
            this.tLPCustomButton2.Controls.Add(this.btnCustom12, 2, 3);
            this.tLPCustomButton2.Controls.Add(this.btnCustom13, 3, 0);
            this.tLPCustomButton2.Controls.Add(this.btnCustom14, 3, 1);
            this.tLPCustomButton2.Controls.Add(this.btnCustom15, 3, 2);
            this.tLPCustomButton2.Controls.Add(this.btnCustom16, 3, 3);
            this.tLPCustomButton2.Name = "tLPCustomButton2";
            // 
            // btnCustom1
            // 
            resources.ApplyResources(this.btnCustom1, "btnCustom1");
            this.btnCustom1.Name = "btnCustom1";
            this.btnCustom1.UseVisualStyleBackColor = true;
            this.btnCustom1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom2
            // 
            resources.ApplyResources(this.btnCustom2, "btnCustom2");
            this.btnCustom2.Name = "btnCustom2";
            this.btnCustom2.UseVisualStyleBackColor = true;
            this.btnCustom2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom3
            // 
            resources.ApplyResources(this.btnCustom3, "btnCustom3");
            this.btnCustom3.Name = "btnCustom3";
            this.btnCustom3.UseVisualStyleBackColor = true;
            this.btnCustom3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom4
            // 
            resources.ApplyResources(this.btnCustom4, "btnCustom4");
            this.btnCustom4.Name = "btnCustom4";
            this.btnCustom4.UseVisualStyleBackColor = true;
            this.btnCustom4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom5
            // 
            resources.ApplyResources(this.btnCustom5, "btnCustom5");
            this.btnCustom5.Name = "btnCustom5";
            this.btnCustom5.UseVisualStyleBackColor = true;
            this.btnCustom5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom6
            // 
            resources.ApplyResources(this.btnCustom6, "btnCustom6");
            this.btnCustom6.Name = "btnCustom6";
            this.btnCustom6.UseVisualStyleBackColor = true;
            this.btnCustom6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom7
            // 
            resources.ApplyResources(this.btnCustom7, "btnCustom7");
            this.btnCustom7.Name = "btnCustom7";
            this.btnCustom7.UseVisualStyleBackColor = true;
            this.btnCustom7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom8
            // 
            resources.ApplyResources(this.btnCustom8, "btnCustom8");
            this.btnCustom8.Name = "btnCustom8";
            this.btnCustom8.UseVisualStyleBackColor = true;
            this.btnCustom8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom9
            // 
            resources.ApplyResources(this.btnCustom9, "btnCustom9");
            this.btnCustom9.Name = "btnCustom9";
            this.btnCustom9.UseVisualStyleBackColor = true;
            this.btnCustom9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom10
            // 
            resources.ApplyResources(this.btnCustom10, "btnCustom10");
            this.btnCustom10.Name = "btnCustom10";
            this.btnCustom10.UseVisualStyleBackColor = true;
            this.btnCustom10.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom11
            // 
            resources.ApplyResources(this.btnCustom11, "btnCustom11");
            this.btnCustom11.Name = "btnCustom11";
            this.btnCustom11.UseVisualStyleBackColor = true;
            this.btnCustom11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom12
            // 
            resources.ApplyResources(this.btnCustom12, "btnCustom12");
            this.btnCustom12.Name = "btnCustom12";
            this.btnCustom12.UseVisualStyleBackColor = true;
            this.btnCustom12.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom13
            // 
            resources.ApplyResources(this.btnCustom13, "btnCustom13");
            this.btnCustom13.Name = "btnCustom13";
            this.btnCustom13.UseVisualStyleBackColor = true;
            this.btnCustom13.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom14
            // 
            resources.ApplyResources(this.btnCustom14, "btnCustom14");
            this.btnCustom14.Name = "btnCustom14";
            this.btnCustom14.UseVisualStyleBackColor = true;
            this.btnCustom14.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom15
            // 
            resources.ApplyResources(this.btnCustom15, "btnCustom15");
            this.btnCustom15.Name = "btnCustom15";
            this.btnCustom15.UseVisualStyleBackColor = true;
            this.btnCustom15.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // btnCustom16
            // 
            resources.ApplyResources(this.btnCustom16, "btnCustom16");
            this.btnCustom16.Name = "btnCustom16";
            this.btnCustom16.UseVisualStyleBackColor = true;
            this.btnCustom16.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnCustomButton_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // groupBoxCoordinates
            // 
            this.groupBoxCoordinates.Controls.Add(this.label_c);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroC);
            this.groupBoxCoordinates.Controls.Add(this.label_mc);
            this.groupBoxCoordinates.Controls.Add(this.label_wc);
            this.groupBoxCoordinates.Controls.Add(this.label11);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroB);
            this.groupBoxCoordinates.Controls.Add(this.label_mb);
            this.groupBoxCoordinates.Controls.Add(this.label_wb);
            this.groupBoxCoordinates.Controls.Add(this.lblCurrentG);
            this.groupBoxCoordinates.Controls.Add(this.label_status0);
            this.groupBoxCoordinates.Controls.Add(this.label_a);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroA);
            this.groupBoxCoordinates.Controls.Add(this.label_ma);
            this.groupBoxCoordinates.Controls.Add(this.label_wa);
            this.groupBoxCoordinates.Controls.Add(this.btnHome);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroXYZ);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroXY);
            this.groupBoxCoordinates.Controls.Add(this.label4);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroZ);
            this.groupBoxCoordinates.Controls.Add(this.label3);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroY);
            this.groupBoxCoordinates.Controls.Add(this.label2);
            this.groupBoxCoordinates.Controls.Add(this.btnZeroX);
            this.groupBoxCoordinates.Controls.Add(this.label_status);
            this.groupBoxCoordinates.Controls.Add(this.label_mx);
            this.groupBoxCoordinates.Controls.Add(this.label_my);
            this.groupBoxCoordinates.Controls.Add(this.label_mz);
            this.groupBoxCoordinates.Controls.Add(this.label_wz);
            this.groupBoxCoordinates.Controls.Add(this.label_wx);
            this.groupBoxCoordinates.Controls.Add(this.label_wy);
            resources.ApplyResources(this.groupBoxCoordinates, "groupBoxCoordinates");
            this.groupBoxCoordinates.Name = "groupBoxCoordinates";
            this.groupBoxCoordinates.TabStop = false;
            // 
            // label_c
            // 
            resources.ApplyResources(this.label_c, "label_c");
            this.label_c.Name = "label_c";
            // 
            // btnZeroC
            // 
            resources.ApplyResources(this.btnZeroC, "btnZeroC");
            this.btnZeroC.Name = "btnZeroC";
            this.toolTip1.SetToolTip(this.btnZeroC, resources.GetString("btnZeroC.ToolTip"));
            this.btnZeroC.UseVisualStyleBackColor = true;
            this.btnZeroC.Click += new System.EventHandler(this.btnZeroC_Click);
            // 
            // label_mc
            // 
            resources.ApplyResources(this.label_mc, "label_mc");
            this.label_mc.Name = "label_mc";
            // 
            // label_wc
            // 
            resources.ApplyResources(this.label_wc, "label_wc");
            this.label_wc.Name = "label_wc";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // btnZeroB
            // 
            resources.ApplyResources(this.btnZeroB, "btnZeroB");
            this.btnZeroB.Name = "btnZeroB";
            this.toolTip1.SetToolTip(this.btnZeroB, resources.GetString("btnZeroB.ToolTip"));
            this.btnZeroB.UseVisualStyleBackColor = true;
            this.btnZeroB.Click += new System.EventHandler(this.btnZeroB_Click);
            // 
            // label_mb
            // 
            resources.ApplyResources(this.label_mb, "label_mb");
            this.label_mb.Name = "label_mb";
            // 
            // label_wb
            // 
            resources.ApplyResources(this.label_wb, "label_wb");
            this.label_wb.Name = "label_wb";
            // 
            // lblCurrentG
            // 
            resources.ApplyResources(this.lblCurrentG, "lblCurrentG");
            this.lblCurrentG.Name = "lblCurrentG";
            this.toolTip1.SetToolTip(this.lblCurrentG, resources.GetString("lblCurrentG.ToolTip"));
            // 
            // label_status0
            // 
            resources.ApplyResources(this.label_status0, "label_status0");
            this.label_status0.Name = "label_status0";
            // 
            // label_a
            // 
            resources.ApplyResources(this.label_a, "label_a");
            this.label_a.Name = "label_a";
            // 
            // btnZeroA
            // 
            resources.ApplyResources(this.btnZeroA, "btnZeroA");
            this.btnZeroA.Name = "btnZeroA";
            this.toolTip1.SetToolTip(this.btnZeroA, resources.GetString("btnZeroA.ToolTip"));
            this.btnZeroA.UseVisualStyleBackColor = true;
            this.btnZeroA.Click += new System.EventHandler(this.btnZeroA_Click);
            // 
            // label_ma
            // 
            resources.ApplyResources(this.label_ma, "label_ma");
            this.label_ma.Name = "label_ma";
            // 
            // label_wa
            // 
            resources.ApplyResources(this.label_wa, "label_wa");
            this.label_wa.Name = "label_wa";
            // 
            // btnHome
            // 
            resources.ApplyResources(this.btnHome, "btnHome");
            this.btnHome.Name = "btnHome";
            this.toolTip1.SetToolTip(this.btnHome, resources.GetString("btnHome.ToolTip"));
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnZeroXYZ
            // 
            resources.ApplyResources(this.btnZeroXYZ, "btnZeroXYZ");
            this.btnZeroXYZ.Name = "btnZeroXYZ";
            this.toolTip1.SetToolTip(this.btnZeroXYZ, resources.GetString("btnZeroXYZ.ToolTip"));
            this.btnZeroXYZ.UseVisualStyleBackColor = true;
            this.btnZeroXYZ.Click += new System.EventHandler(this.btnZeroXYZ_Click);
            // 
            // btnZeroXY
            // 
            resources.ApplyResources(this.btnZeroXY, "btnZeroXY");
            this.btnZeroXY.Name = "btnZeroXY";
            this.toolTip1.SetToolTip(this.btnZeroXY, resources.GetString("btnZeroXY.ToolTip"));
            this.btnZeroXY.UseVisualStyleBackColor = true;
            this.btnZeroXY.Click += new System.EventHandler(this.btnZeroXY_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // btnZeroZ
            // 
            resources.ApplyResources(this.btnZeroZ, "btnZeroZ");
            this.btnZeroZ.Name = "btnZeroZ";
            this.toolTip1.SetToolTip(this.btnZeroZ, resources.GetString("btnZeroZ.ToolTip"));
            this.btnZeroZ.UseVisualStyleBackColor = true;
            this.btnZeroZ.Click += new System.EventHandler(this.btnZeroZ_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // btnZeroY
            // 
            resources.ApplyResources(this.btnZeroY, "btnZeroY");
            this.btnZeroY.Name = "btnZeroY";
            this.toolTip1.SetToolTip(this.btnZeroY, resources.GetString("btnZeroY.ToolTip"));
            this.btnZeroY.UseVisualStyleBackColor = true;
            this.btnZeroY.Click += new System.EventHandler(this.btnZeroY_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // btnZeroX
            // 
            resources.ApplyResources(this.btnZeroX, "btnZeroX");
            this.btnZeroX.Name = "btnZeroX";
            this.toolTip1.SetToolTip(this.btnZeroX, resources.GetString("btnZeroX.ToolTip"));
            this.btnZeroX.UseVisualStyleBackColor = true;
            this.btnZeroX.Click += new System.EventHandler(this.btnZeroX_Click);
            // 
            // label_status
            // 
            resources.ApplyResources(this.label_status, "label_status");
            this.label_status.Name = "label_status";
            // 
            // label_mx
            // 
            resources.ApplyResources(this.label_mx, "label_mx");
            this.label_mx.Name = "label_mx";
            // 
            // label_my
            // 
            resources.ApplyResources(this.label_my, "label_my");
            this.label_my.Name = "label_my";
            // 
            // label_mz
            // 
            resources.ApplyResources(this.label_mz, "label_mz");
            this.label_mz.Name = "label_mz";
            // 
            // label_wz
            // 
            resources.ApplyResources(this.label_wz, "label_wz");
            this.label_wz.Name = "label_wz";
            // 
            // label_wx
            // 
            resources.ApplyResources(this.label_wx, "label_wx");
            this.label_wx.Name = "label_wx";
            // 
            // label_wy
            // 
            resources.ApplyResources(this.label_wy, "label_wy");
            this.label_wy.Name = "label_wy";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.createGCodeToolStripMenuItem,
            this.gCodeToolStripMenuItem,
            this.workpieceToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.logToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.toolStripMenuItem2,
            this.toolStripSeparator2,
            this.saveToolStripMenuItem,
            this.toolStripSeparator3,
            this.setupToolStripMenuItem1,
            this.toolStripSeparator7,
            this.saveMachineParametersToolStripMenuItem,
            this.loadMachineParametersToolStripMenuItem,
            this.toolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            resources.ApplyResources(this.loadToolStripMenuItem, "loadToolStripMenuItem");
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.btnSaveFile_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // setupToolStripMenuItem1
            // 
            this.setupToolStripMenuItem1.Name = "setupToolStripMenuItem1";
            resources.ApplyResources(this.setupToolStripMenuItem1, "setupToolStripMenuItem1");
            this.setupToolStripMenuItem1.Click += new System.EventHandler(this.setupToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // saveMachineParametersToolStripMenuItem
            // 
            this.saveMachineParametersToolStripMenuItem.Name = "saveMachineParametersToolStripMenuItem";
            resources.ApplyResources(this.saveMachineParametersToolStripMenuItem, "saveMachineParametersToolStripMenuItem");
            this.saveMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.saveMachineParametersToolStripMenuItem_Click);
            // 
            // loadMachineParametersToolStripMenuItem
            // 
            this.loadMachineParametersToolStripMenuItem.Name = "loadMachineParametersToolStripMenuItem";
            resources.ApplyResources(this.loadMachineParametersToolStripMenuItem, "loadMachineParametersToolStripMenuItem");
            this.loadMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.loadMachineParametersToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.AutoToolTip = true;
            this.toolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.deutschToolStripMenuItem,
            this.pусскийToolStripMenuItem,
            this.toolStripMenuItem4,
            this.portuguêsToolStripMenuItem,
            this.franzToolStripMenuItem,
            this.chinesischToolStripMenuItem,
            this.arabischToolStripMenuItem,
            this.japanischToolStripMenuItem});
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // deutschToolStripMenuItem
            // 
            this.deutschToolStripMenuItem.Name = "deutschToolStripMenuItem";
            resources.ApplyResources(this.deutschToolStripMenuItem, "deutschToolStripMenuItem");
            this.deutschToolStripMenuItem.Click += new System.EventHandler(this.deutschToolStripMenuItem_Click);
            // 
            // pусскийToolStripMenuItem
            // 
            this.pусскийToolStripMenuItem.Name = "pусскийToolStripMenuItem";
            resources.ApplyResources(this.pусскийToolStripMenuItem, "pусскийToolStripMenuItem");
            this.pусскийToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Click += new System.EventHandler(this.spanToolStripMenuItem_Click);
            // 
            // portuguêsToolStripMenuItem
            // 
            this.portuguêsToolStripMenuItem.Name = "portuguêsToolStripMenuItem";
            resources.ApplyResources(this.portuguêsToolStripMenuItem, "portuguêsToolStripMenuItem");
            this.portuguêsToolStripMenuItem.Click += new System.EventHandler(this.portugisischToolStripMenuItem_Click);
            // 
            // franzToolStripMenuItem
            // 
            this.franzToolStripMenuItem.Name = "franzToolStripMenuItem";
            resources.ApplyResources(this.franzToolStripMenuItem, "franzToolStripMenuItem");
            this.franzToolStripMenuItem.Click += new System.EventHandler(this.franzToolStripMenuItem_Click);
            // 
            // chinesischToolStripMenuItem
            // 
            this.chinesischToolStripMenuItem.Name = "chinesischToolStripMenuItem";
            resources.ApplyResources(this.chinesischToolStripMenuItem, "chinesischToolStripMenuItem");
            this.chinesischToolStripMenuItem.Click += new System.EventHandler(this.chinesischToolStripMenuItem_Click);
            // 
            // arabischToolStripMenuItem
            // 
            this.arabischToolStripMenuItem.Name = "arabischToolStripMenuItem";
            resources.ApplyResources(this.arabischToolStripMenuItem, "arabischToolStripMenuItem");
            this.arabischToolStripMenuItem.Click += new System.EventHandler(this.arabischToolStripMenuItem_Click);
            // 
            // japanischToolStripMenuItem
            // 
            this.japanischToolStripMenuItem.Name = "japanischToolStripMenuItem";
            resources.ApplyResources(this.japanischToolStripMenuItem, "japanischToolStripMenuItem");
            this.japanischToolStripMenuItem.Click += new System.EventHandler(this.japanischToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // createGCodeToolStripMenuItem
            // 
            this.createGCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textWizzardToolStripMenuItem,
            this.createBarcodeToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.createSimpleShapesToolStripMenuItem,
            this.createJogPathToolStripMenuItem,
            this.startExtensionToolStripMenuItem});
            this.createGCodeToolStripMenuItem.Name = "createGCodeToolStripMenuItem";
            resources.ApplyResources(this.createGCodeToolStripMenuItem, "createGCodeToolStripMenuItem");
            // 
            // textWizzardToolStripMenuItem
            // 
            this.textWizzardToolStripMenuItem.Name = "textWizzardToolStripMenuItem";
            resources.ApplyResources(this.textWizzardToolStripMenuItem, "textWizzardToolStripMenuItem");
            this.textWizzardToolStripMenuItem.Click += new System.EventHandler(this.textWizzardToolStripMenuItem_Click);
            // 
            // createBarcodeToolStripMenuItem
            // 
            this.createBarcodeToolStripMenuItem.Name = "createBarcodeToolStripMenuItem";
            resources.ApplyResources(this.createBarcodeToolStripMenuItem, "createBarcodeToolStripMenuItem");
            this.createBarcodeToolStripMenuItem.Click += new System.EventHandler(this.createBarcodeToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            resources.ApplyResources(this.imageToolStripMenuItem, "imageToolStripMenuItem");
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.imageToolStripMenuItem_Click);
            // 
            // createSimpleShapesToolStripMenuItem
            // 
            this.createSimpleShapesToolStripMenuItem.Name = "createSimpleShapesToolStripMenuItem";
            resources.ApplyResources(this.createSimpleShapesToolStripMenuItem, "createSimpleShapesToolStripMenuItem");
            this.createSimpleShapesToolStripMenuItem.Click += new System.EventHandler(this.createSimpleShapesToolStripMenuItem_Click);
            // 
            // createJogPathToolStripMenuItem
            // 
            this.createJogPathToolStripMenuItem.Name = "createJogPathToolStripMenuItem";
            resources.ApplyResources(this.createJogPathToolStripMenuItem, "createJogPathToolStripMenuItem");
            this.createJogPathToolStripMenuItem.Click += new System.EventHandler(this.jogCreatorToolStripMenuItem_Click);
            // 
            // startExtensionToolStripMenuItem
            // 
            this.startExtensionToolStripMenuItem.Name = "startExtensionToolStripMenuItem";
            resources.ApplyResources(this.startExtensionToolStripMenuItem, "startExtensionToolStripMenuItem");
            // 
            // gCodeToolStripMenuItem
            // 
            this.gCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unDoToolStripMenuItem,
            this.toolStripSeparator16,
            this.mirrorXToolStripMenuItem,
            this.mirrorYToolStripMenuItem,
            this.mirrorRotaryToolStripMenuItem,
            this.toolStripSeparator4,
            this.rotate90ToolStripMenuItem,
            this.rotate90ToolStripMenuItem1,
            this.rotate180ToolStripMenuItem,
            this.rotateFreeToolStripMenuItem,
            this.toolStripSeparator5,
            this.sToolStripMenuItem,
            this.skalierenXYToolStripMenuItem,
            this.skalierenXYUmXUnitsZuErreichenToolStripMenuItem,
            this.skaliereXUmToolStripMenuItem,
            this.skaliereAufXUnitsToolStripMenuItem,
            this.skaliereYUmToolStripMenuItem,
            this.skaliereAufYUnitsToolStripMenuItem,
            this.toolStripSeparator6,
            this.rotaryDimaeterToolStripMenuItem,
            this.skaliereXAufDrehachseToolStripMenuItem,
            this.skaliereYAufDrehachseToolStripMenuItem,
            this.toolStripSeparator15,
            this.toolStrip_RadiusComp,
            this.ersetzteG23DurchLinienToolStripMenuItem,
            this.convertZToSspindleSpeedToolStripMenuItem,
            this.removeAnyZMoveToolStripMenuItem});
            this.gCodeToolStripMenuItem.Name = "gCodeToolStripMenuItem";
            resources.ApplyResources(this.gCodeToolStripMenuItem, "gCodeToolStripMenuItem");
            // 
            // unDoToolStripMenuItem
            // 
            resources.ApplyResources(this.unDoToolStripMenuItem, "unDoToolStripMenuItem");
            this.unDoToolStripMenuItem.Name = "unDoToolStripMenuItem";
            this.unDoToolStripMenuItem.Click += new System.EventHandler(this.unDoToolStripMenuItem_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // mirrorXToolStripMenuItem
            // 
            this.mirrorXToolStripMenuItem.Name = "mirrorXToolStripMenuItem";
            resources.ApplyResources(this.mirrorXToolStripMenuItem, "mirrorXToolStripMenuItem");
            this.mirrorXToolStripMenuItem.Click += new System.EventHandler(this.mirrorXToolStripMenuItem_Click);
            // 
            // mirrorYToolStripMenuItem
            // 
            this.mirrorYToolStripMenuItem.Name = "mirrorYToolStripMenuItem";
            resources.ApplyResources(this.mirrorYToolStripMenuItem, "mirrorYToolStripMenuItem");
            this.mirrorYToolStripMenuItem.Click += new System.EventHandler(this.mirrorYToolStripMenuItem_Click);
            // 
            // mirrorRotaryToolStripMenuItem
            // 
            this.mirrorRotaryToolStripMenuItem.Name = "mirrorRotaryToolStripMenuItem";
            resources.ApplyResources(this.mirrorRotaryToolStripMenuItem, "mirrorRotaryToolStripMenuItem");
            this.mirrorRotaryToolStripMenuItem.Click += new System.EventHandler(this.mirrorRotaryToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // rotate90ToolStripMenuItem
            // 
            this.rotate90ToolStripMenuItem.Name = "rotate90ToolStripMenuItem";
            resources.ApplyResources(this.rotate90ToolStripMenuItem, "rotate90ToolStripMenuItem");
            this.rotate90ToolStripMenuItem.Click += new System.EventHandler(this.rotate90ToolStripMenuItem_Click);
            // 
            // rotate90ToolStripMenuItem1
            // 
            this.rotate90ToolStripMenuItem1.Name = "rotate90ToolStripMenuItem1";
            resources.ApplyResources(this.rotate90ToolStripMenuItem1, "rotate90ToolStripMenuItem1");
            this.rotate90ToolStripMenuItem1.Click += new System.EventHandler(this.rotate90ToolStripMenuItem1_Click);
            // 
            // rotate180ToolStripMenuItem
            // 
            this.rotate180ToolStripMenuItem.Name = "rotate180ToolStripMenuItem";
            resources.ApplyResources(this.rotate180ToolStripMenuItem, "rotate180ToolStripMenuItem");
            this.rotate180ToolStripMenuItem.Click += new System.EventHandler(this.rotate180ToolStripMenuItem_Click);
            // 
            // rotateFreeToolStripMenuItem
            // 
            this.rotateFreeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_rotate});
            this.rotateFreeToolStripMenuItem.Name = "rotateFreeToolStripMenuItem";
            resources.ApplyResources(this.rotateFreeToolStripMenuItem, "rotateFreeToolStripMenuItem");
            // 
            // toolStrip_tb_rotate
            // 
            this.toolStrip_tb_rotate.Name = "toolStrip_tb_rotate";
            resources.ApplyResources(this.toolStrip_tb_rotate, "toolStrip_tb_rotate");
            this.toolStrip_tb_rotate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_rotate_KeyDown);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // sToolStripMenuItem
            // 
            this.sToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_XY_scale});
            this.sToolStripMenuItem.Name = "sToolStripMenuItem";
            resources.ApplyResources(this.sToolStripMenuItem, "sToolStripMenuItem");
            // 
            // toolStrip_tb_XY_scale
            // 
            this.toolStrip_tb_XY_scale.Name = "toolStrip_tb_XY_scale";
            resources.ApplyResources(this.toolStrip_tb_XY_scale, "toolStrip_tb_XY_scale");
            this.toolStrip_tb_XY_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_XY_scale_KeyDown);
            // 
            // skalierenXYToolStripMenuItem
            // 
            this.skalierenXYToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_XY_X_scale});
            this.skalierenXYToolStripMenuItem.Name = "skalierenXYToolStripMenuItem";
            resources.ApplyResources(this.skalierenXYToolStripMenuItem, "skalierenXYToolStripMenuItem");
            // 
            // toolStrip_tb_XY_X_scale
            // 
            this.toolStrip_tb_XY_X_scale.Name = "toolStrip_tb_XY_X_scale";
            resources.ApplyResources(this.toolStrip_tb_XY_X_scale, "toolStrip_tb_XY_X_scale");
            this.toolStrip_tb_XY_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_XY_X_scale_KeyDown);
            // 
            // skalierenXYUmXUnitsZuErreichenToolStripMenuItem
            // 
            this.skalierenXYUmXUnitsZuErreichenToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_XY_Y_scale});
            this.skalierenXYUmXUnitsZuErreichenToolStripMenuItem.Name = "skalierenXYUmXUnitsZuErreichenToolStripMenuItem";
            resources.ApplyResources(this.skalierenXYUmXUnitsZuErreichenToolStripMenuItem, "skalierenXYUmXUnitsZuErreichenToolStripMenuItem");
            // 
            // toolStrip_tb_XY_Y_scale
            // 
            this.toolStrip_tb_XY_Y_scale.Name = "toolStrip_tb_XY_Y_scale";
            resources.ApplyResources(this.toolStrip_tb_XY_Y_scale, "toolStrip_tb_XY_Y_scale");
            this.toolStrip_tb_XY_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_XY_Y_scale_KeyDown);
            // 
            // skaliereXUmToolStripMenuItem
            // 
            this.skaliereXUmToolStripMenuItem.AutoToolTip = true;
            this.skaliereXUmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_X_scale});
            this.skaliereXUmToolStripMenuItem.Name = "skaliereXUmToolStripMenuItem";
            resources.ApplyResources(this.skaliereXUmToolStripMenuItem, "skaliereXUmToolStripMenuItem");
            // 
            // toolStrip_tb_X_scale
            // 
            this.toolStrip_tb_X_scale.Name = "toolStrip_tb_X_scale";
            resources.ApplyResources(this.toolStrip_tb_X_scale, "toolStrip_tb_X_scale");
            this.toolStrip_tb_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_X_scale_KeyDown);
            // 
            // skaliereAufXUnitsToolStripMenuItem
            // 
            this.skaliereAufXUnitsToolStripMenuItem.AutoToolTip = true;
            this.skaliereAufXUnitsToolStripMenuItem.BackColor = System.Drawing.SystemColors.Control;
            this.skaliereAufXUnitsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_X_X_scale});
            this.skaliereAufXUnitsToolStripMenuItem.Name = "skaliereAufXUnitsToolStripMenuItem";
            resources.ApplyResources(this.skaliereAufXUnitsToolStripMenuItem, "skaliereAufXUnitsToolStripMenuItem");
            // 
            // toolStrip_tb_X_X_scale
            // 
            this.toolStrip_tb_X_X_scale.Name = "toolStrip_tb_X_X_scale";
            resources.ApplyResources(this.toolStrip_tb_X_X_scale, "toolStrip_tb_X_X_scale");
            this.toolStrip_tb_X_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_X_X_scale_KeyDown);
            // 
            // skaliereYUmToolStripMenuItem
            // 
            this.skaliereYUmToolStripMenuItem.AutoToolTip = true;
            this.skaliereYUmToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_Y_scale});
            this.skaliereYUmToolStripMenuItem.Name = "skaliereYUmToolStripMenuItem";
            resources.ApplyResources(this.skaliereYUmToolStripMenuItem, "skaliereYUmToolStripMenuItem");
            // 
            // toolStrip_tb_Y_scale
            // 
            this.toolStrip_tb_Y_scale.Name = "toolStrip_tb_Y_scale";
            resources.ApplyResources(this.toolStrip_tb_Y_scale, "toolStrip_tb_Y_scale");
            this.toolStrip_tb_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_Y_scale_KeyDown);
            // 
            // skaliereAufYUnitsToolStripMenuItem
            // 
            this.skaliereAufYUnitsToolStripMenuItem.AutoToolTip = true;
            this.skaliereAufYUnitsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_Y_Y_scale});
            this.skaliereAufYUnitsToolStripMenuItem.Name = "skaliereAufYUnitsToolStripMenuItem";
            resources.ApplyResources(this.skaliereAufYUnitsToolStripMenuItem, "skaliereAufYUnitsToolStripMenuItem");
            // 
            // toolStrip_tb_Y_Y_scale
            // 
            this.toolStrip_tb_Y_Y_scale.Name = "toolStrip_tb_Y_Y_scale";
            resources.ApplyResources(this.toolStrip_tb_Y_Y_scale, "toolStrip_tb_Y_Y_scale");
            this.toolStrip_tb_Y_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_Y_Y_scale_KeyDown);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // rotaryDimaeterToolStripMenuItem
            // 
            this.rotaryDimaeterToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_rotary_diameter});
            this.rotaryDimaeterToolStripMenuItem.Name = "rotaryDimaeterToolStripMenuItem";
            resources.ApplyResources(this.rotaryDimaeterToolStripMenuItem, "rotaryDimaeterToolStripMenuItem");
            // 
            // toolStrip_tb_rotary_diameter
            // 
            this.toolStrip_tb_rotary_diameter.Name = "toolStrip_tb_rotary_diameter";
            resources.ApplyResources(this.toolStrip_tb_rotary_diameter, "toolStrip_tb_rotary_diameter");
            this.toolStrip_tb_rotary_diameter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_rotary_diameter_KeyDown);
            // 
            // skaliereXAufDrehachseToolStripMenuItem
            // 
            this.skaliereXAufDrehachseToolStripMenuItem.AutoToolTip = true;
            this.skaliereXAufDrehachseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_X_A_scale});
            this.skaliereXAufDrehachseToolStripMenuItem.Name = "skaliereXAufDrehachseToolStripMenuItem";
            resources.ApplyResources(this.skaliereXAufDrehachseToolStripMenuItem, "skaliereXAufDrehachseToolStripMenuItem");
            // 
            // toolStrip_tb_X_A_scale
            // 
            this.toolStrip_tb_X_A_scale.Name = "toolStrip_tb_X_A_scale";
            resources.ApplyResources(this.toolStrip_tb_X_A_scale, "toolStrip_tb_X_A_scale");
            this.toolStrip_tb_X_A_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_X_A_scale_KeyDown);
            // 
            // skaliereYAufDrehachseToolStripMenuItem
            // 
            this.skaliereYAufDrehachseToolStripMenuItem.AutoToolTip = true;
            this.skaliereYAufDrehachseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_Y_A_scale});
            this.skaliereYAufDrehachseToolStripMenuItem.Name = "skaliereYAufDrehachseToolStripMenuItem";
            resources.ApplyResources(this.skaliereYAufDrehachseToolStripMenuItem, "skaliereYAufDrehachseToolStripMenuItem");
            // 
            // toolStrip_tb_Y_A_scale
            // 
            this.toolStrip_tb_Y_A_scale.Name = "toolStrip_tb_Y_A_scale";
            resources.ApplyResources(this.toolStrip_tb_Y_A_scale, "toolStrip_tb_Y_A_scale");
            this.toolStrip_tb_Y_A_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_Y_A_scale_KeyDown);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // toolStrip_RadiusComp
            // 
            this.toolStrip_RadiusComp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tBRadiusCompValue});
            this.toolStrip_RadiusComp.Name = "toolStrip_RadiusComp";
            resources.ApplyResources(this.toolStrip_RadiusComp, "toolStrip_RadiusComp");
            // 
            // toolStrip_tBRadiusCompValue
            // 
            this.toolStrip_tBRadiusCompValue.Name = "toolStrip_tBRadiusCompValue";
            resources.ApplyResources(this.toolStrip_tBRadiusCompValue, "toolStrip_tBRadiusCompValue");
            this.toolStrip_tBRadiusCompValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tBRadiusCompValue_KeyDown);
            // 
            // ersetzteG23DurchLinienToolStripMenuItem
            // 
            this.ersetzteG23DurchLinienToolStripMenuItem.Name = "ersetzteG23DurchLinienToolStripMenuItem";
            resources.ApplyResources(this.ersetzteG23DurchLinienToolStripMenuItem, "ersetzteG23DurchLinienToolStripMenuItem");
            this.ersetzteG23DurchLinienToolStripMenuItem.Click += new System.EventHandler(this.ersetzteG23DurchLinienToolStripMenuItem_Click);
            // 
            // convertZToSspindleSpeedToolStripMenuItem
            // 
            this.convertZToSspindleSpeedToolStripMenuItem.Name = "convertZToSspindleSpeedToolStripMenuItem";
            resources.ApplyResources(this.convertZToSspindleSpeedToolStripMenuItem, "convertZToSspindleSpeedToolStripMenuItem");
            this.convertZToSspindleSpeedToolStripMenuItem.Click += new System.EventHandler(this.convertZToSspindleSpeedToolStripMenuItem_Click);
            // 
            // removeAnyZMoveToolStripMenuItem
            // 
            this.removeAnyZMoveToolStripMenuItem.Name = "removeAnyZMoveToolStripMenuItem";
            resources.ApplyResources(this.removeAnyZMoveToolStripMenuItem, "removeAnyZMoveToolStripMenuItem");
            this.removeAnyZMoveToolStripMenuItem.Click += new System.EventHandler(this.removeAnyZMoveToolStripMenuItem_Click);
            // 
            // workpieceToolStripMenuItem
            // 
            this.workpieceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.probingToolLengthToolStripMenuItem,
            this.surfaceScanHeightMapToolStripMenuItem,
            this.jogPathCreatorToolStripMenuItem});
            this.workpieceToolStripMenuItem.Name = "workpieceToolStripMenuItem";
            resources.ApplyResources(this.workpieceToolStripMenuItem, "workpieceToolStripMenuItem");
            // 
            // probingToolLengthToolStripMenuItem
            // 
            this.probingToolLengthToolStripMenuItem.Name = "probingToolLengthToolStripMenuItem";
            resources.ApplyResources(this.probingToolLengthToolStripMenuItem, "probingToolLengthToolStripMenuItem");
            this.probingToolLengthToolStripMenuItem.Click += new System.EventHandler(this.edgeFinderopen);
            // 
            // surfaceScanHeightMapToolStripMenuItem
            // 
            this.surfaceScanHeightMapToolStripMenuItem.Name = "surfaceScanHeightMapToolStripMenuItem";
            resources.ApplyResources(this.surfaceScanHeightMapToolStripMenuItem, "surfaceScanHeightMapToolStripMenuItem");
            this.surfaceScanHeightMapToolStripMenuItem.Click += new System.EventHandler(this.heightMapToolStripMenuItem_Click);
            // 
            // jogPathCreatorToolStripMenuItem
            // 
            this.jogPathCreatorToolStripMenuItem.Name = "jogPathCreatorToolStripMenuItem";
            resources.ApplyResources(this.jogPathCreatorToolStripMenuItem, "jogPathCreatorToolStripMenuItem");
            this.jogPathCreatorToolStripMenuItem.Click += new System.EventHandler(this.jogCreatorToolStripMenuItem_Click);
            // 
            // machineToolStripMenuItem
            // 
            this.machineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.edgeFinderToolStripMenuItem,
            this.heightMapToolStripMenuItem,
            this.laserToolsToolStripMenuItem,
            this.coordinateSystemsToolStripMenuItem,
            this.setupToolStripMenuItem,
            this.toolStripMenuItem1,
            this.startStreamingAtLineToolStripMenuItem,
            this.controlStreamingToolStripMenuItem,
            this.control2ndGRBLToolStripMenuItem,
            this.control3rdGRBLToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            resources.ApplyResources(this.machineToolStripMenuItem, "machineToolStripMenuItem");
            // 
            // edgeFinderToolStripMenuItem
            // 
            this.edgeFinderToolStripMenuItem.Name = "edgeFinderToolStripMenuItem";
            resources.ApplyResources(this.edgeFinderToolStripMenuItem, "edgeFinderToolStripMenuItem");
            this.edgeFinderToolStripMenuItem.Click += new System.EventHandler(this.edgeFinderopen);
            // 
            // heightMapToolStripMenuItem
            // 
            this.heightMapToolStripMenuItem.Name = "heightMapToolStripMenuItem";
            resources.ApplyResources(this.heightMapToolStripMenuItem, "heightMapToolStripMenuItem");
            this.heightMapToolStripMenuItem.Click += new System.EventHandler(this.heightMapToolStripMenuItem_Click);
            // 
            // laserToolsToolStripMenuItem
            // 
            this.laserToolsToolStripMenuItem.Name = "laserToolsToolStripMenuItem";
            resources.ApplyResources(this.laserToolsToolStripMenuItem, "laserToolsToolStripMenuItem");
            this.laserToolsToolStripMenuItem.Click += new System.EventHandler(this.laseropen);
            // 
            // coordinateSystemsToolStripMenuItem
            // 
            this.coordinateSystemsToolStripMenuItem.Name = "coordinateSystemsToolStripMenuItem";
            resources.ApplyResources(this.coordinateSystemsToolStripMenuItem, "coordinateSystemsToolStripMenuItem");
            this.coordinateSystemsToolStripMenuItem.Click += new System.EventHandler(this.coordSystemopen);
            // 
            // setupToolStripMenuItem
            // 
            this.setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            resources.ApplyResources(this.setupToolStripMenuItem, "setupToolStripMenuItem");
            this.setupToolStripMenuItem.Click += new System.EventHandler(this.DIYControlopen);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.cameraToolStripMenuItem_Click);
            // 
            // startStreamingAtLineToolStripMenuItem
            // 
            this.startStreamingAtLineToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStrip_tb_StreamLine});
            this.startStreamingAtLineToolStripMenuItem.Name = "startStreamingAtLineToolStripMenuItem";
            resources.ApplyResources(this.startStreamingAtLineToolStripMenuItem, "startStreamingAtLineToolStripMenuItem");
            // 
            // toolStrip_tb_StreamLine
            // 
            this.toolStrip_tb_StreamLine.Name = "toolStrip_tb_StreamLine";
            resources.ApplyResources(this.toolStrip_tb_StreamLine, "toolStrip_tb_StreamLine");
            this.toolStrip_tb_StreamLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStrip_tb_StreamLine_KeyDown);
            // 
            // controlStreamingToolStripMenuItem
            // 
            this.controlStreamingToolStripMenuItem.Name = "controlStreamingToolStripMenuItem";
            resources.ApplyResources(this.controlStreamingToolStripMenuItem, "controlStreamingToolStripMenuItem");
            this.controlStreamingToolStripMenuItem.Click += new System.EventHandler(this.controlStreamingToolStripMenuItem_Click);
            // 
            // control2ndGRBLToolStripMenuItem
            // 
            this.control2ndGRBLToolStripMenuItem.Name = "control2ndGRBLToolStripMenuItem";
            resources.ApplyResources(this.control2ndGRBLToolStripMenuItem, "control2ndGRBLToolStripMenuItem");
            this.control2ndGRBLToolStripMenuItem.Click += new System.EventHandler(this.control2ndGRBLToolStripMenuItem_Click);
            // 
            // control3rdGRBLToolStripMenuItem
            // 
            this.control3rdGRBLToolStripMenuItem.Name = "control3rdGRBLToolStripMenuItem";
            resources.ApplyResources(this.control3rdGRBLToolStripMenuItem, "control3rdGRBLToolStripMenuItem");
            this.control3rdGRBLToolStripMenuItem.Click += new System.EventHandler(this.control3rdSerialCOMToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripViewRuler,
            this.toolStripViewInfo,
            this.toolStripViewPenUp,
            this.toolStripViewMachineFix,
            this.toolStripViewMachine,
            this.toolStripViewDimension,
            this.toolStripViewTool,
            this.toolStripViewBackground});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // toolStripViewRuler
            // 
            this.toolStripViewRuler.Checked = global::GRBL_Plotter.Properties.Settings.Default.gui2DRulerShow;
            this.toolStripViewRuler.CheckOnClick = true;
            this.toolStripViewRuler.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewRuler.Name = "toolStripViewRuler";
            resources.ApplyResources(this.toolStripViewRuler, "toolStripViewRuler");
            this.toolStripViewRuler.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewInfo
            // 
            this.toolStripViewInfo.Checked = global::GRBL_Plotter.Properties.Settings.Default.gui2DInfoShow;
            this.toolStripViewInfo.CheckOnClick = true;
            this.toolStripViewInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewInfo.Name = "toolStripViewInfo";
            resources.ApplyResources(this.toolStripViewInfo, "toolStripViewInfo");
            this.toolStripViewInfo.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewPenUp
            // 
            this.toolStripViewPenUp.Checked = global::GRBL_Plotter.Properties.Settings.Default.gui2DPenUpShow;
            this.toolStripViewPenUp.CheckOnClick = true;
            this.toolStripViewPenUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewPenUp.Name = "toolStripViewPenUp";
            resources.ApplyResources(this.toolStripViewPenUp, "toolStripViewPenUp");
            this.toolStripViewPenUp.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewMachineFix
            // 
            this.toolStripViewMachineFix.CheckOnClick = true;
            this.toolStripViewMachineFix.Name = "toolStripViewMachineFix";
            resources.ApplyResources(this.toolStripViewMachineFix, "toolStripViewMachineFix");
            this.toolStripViewMachineFix.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewMachine
            // 
            this.toolStripViewMachine.Checked = true;
            this.toolStripViewMachine.CheckOnClick = true;
            this.toolStripViewMachine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewMachine.Name = "toolStripViewMachine";
            resources.ApplyResources(this.toolStripViewMachine, "toolStripViewMachine");
            this.toolStripViewMachine.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewDimension
            // 
            this.toolStripViewDimension.Checked = true;
            this.toolStripViewDimension.CheckOnClick = true;
            this.toolStripViewDimension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewDimension.Name = "toolStripViewDimension";
            resources.ApplyResources(this.toolStripViewDimension, "toolStripViewDimension");
            this.toolStripViewDimension.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewTool
            // 
            this.toolStripViewTool.Checked = global::GRBL_Plotter.Properties.Settings.Default.gui2DToolTableShow;
            this.toolStripViewTool.CheckOnClick = true;
            this.toolStripViewTool.Name = "toolStripViewTool";
            resources.ApplyResources(this.toolStripViewTool, "toolStripViewTool");
            this.toolStripViewTool.Click += new System.EventHandler(this.updateView);
            // 
            // toolStripViewBackground
            // 
            this.toolStripViewBackground.Checked = global::GRBL_Plotter.Properties.Settings.Default.guiBackgroundShow;
            this.toolStripViewBackground.CheckOnClick = true;
            this.toolStripViewBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewBackground.Name = "toolStripViewBackground";
            resources.ApplyResources(this.toolStripViewBackground, "toolStripViewBackground");
            this.toolStripViewBackground.Click += new System.EventHandler(this.updateView);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            resources.ApplyResources(this.logToolStripMenuItem, "logToolStripMenuItem");
            // 
            // MainTimer
            // 
            this.MainTimer.Enabled = true;
            this.MainTimer.Interval = 500;
            this.MainTimer.Tick += new System.EventHandler(this.MainTimer_Tick);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // gamePadTimer
            // 
            this.gamePadTimer.Tick += new System.EventHandler(this.gamePadTimer_Tick);
            // 
            // simulationTimer
            // 
            this.simulationTimer.Interval = 50;
            this.simulationTimer.Tick += new System.EventHandler(this.simulationTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel0,
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel0
            // 
            this.toolStripStatusLabel0.AutoToolTip = true;
            this.toolStripStatusLabel0.Name = "toolStripStatusLabel0";
            resources.ApplyResources(this.toolStripStatusLabel0, "toolStripStatusLabel0");
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoToolTip = true;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoToolTip = true;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            // 
            // SplashScreenTimer
            // 
            this.SplashScreenTimer.Interval = 1500;
            this.SplashScreenTimer.Tick += new System.EventHandler(this.SplashScreenTimer_Tick);
            // 
            // loadTimer
            // 
            this.loadTimer.Tick += new System.EventHandler(this.loadTimer_Tick);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Opacity = 0D;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tLPLinks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).EndInit();
            this.cmsFCTB.ResumeLayout(false);
            this.gBoxStream.ResumeLayout(false);
            this.gBoxStream.PerformLayout();
            this.gBoxDimension.ResumeLayout(false);
            this.gBoxDimension.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.gBoxOverride.ResumeLayout(false);
            this.gBOverrideRGB.ResumeLayout(false);
            this.gBOverrideRGB.PerformLayout();
            this.gBOverrideASGB.ResumeLayout(false);
            this.gBOverrideFRGB.ResumeLayout(false);
            this.gBOverrideFRGB.PerformLayout();
            this.gBOverrideSSGB.ResumeLayout(false);
            this.gBOverrideSSGB.PerformLayout();
            this.tLPRechts.ResumeLayout(false);
            this.tLPRechtsUnten.ResumeLayout(false);
            this.tLPRechtsUnten.PerformLayout();
            this.tLPRechtsUntenRechts.ResumeLayout(false);
            this.tLPRechtsUntenRechts.PerformLayout();
            this.gB_Jogging.ResumeLayout(false);
            this.gB_Jogging.PerformLayout();
            this.gB_Jog0.ResumeLayout(false);
            this.gB_Jog0.PerformLayout();
            this.tLPRechtsUntenRechtsMitte.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tLPMitteUnten.ResumeLayout(false);
            this.tLPMitteUnten.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.cmsPictureBox.ResumeLayout(false);
            this.tLPRechtsOben.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tLPCustomButton1.ResumeLayout(false);
            this.tLPCustomButton2.ResumeLayout(false);
            this.groupBoxCoordinates.ResumeLayout(false);
            this.groupBoxCoordinates.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Label label_mx;
        private System.Windows.Forms.Label label_my;
        private System.Windows.Forms.Label label_mz;
        private System.Windows.Forms.Label label_wx;
        private System.Windows.Forms.Label label_wy;
        private System.Windows.Forms.Label label_wz;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.GroupBox gBoxStream;
        private System.Windows.Forms.ProgressBar pbFile;
        private System.Windows.Forms.Label lblElapsed;
        private System.Windows.Forms.Label lblFileProgress;
        private System.Windows.Forms.ProgressBar pbBuffer;
        private System.Windows.Forms.Button btnStreamStart;
        private System.Windows.Forms.TextBox tbFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBoxCoordinates;
        private System.Windows.Forms.Label label_status;
        private System.Windows.Forms.Label lblRemaining;
        private System.Windows.Forms.Label label_status0;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnZeroXYZ;
        private System.Windows.Forms.Button btnZeroXY;
        private System.Windows.Forms.Button btnZeroZ;
        private System.Windows.Forms.Button btnZeroY;
        private System.Windows.Forms.Button btnZeroX;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnCustom1;
        private System.Windows.Forms.Button btnCustom2;
        private System.Windows.Forms.Button btnCustom3;
        private System.Windows.Forms.Button btnCustom4;
        private System.Windows.Forms.Button btnCustom5;
        private System.Windows.Forms.Button btnCustom6;
        private System.Windows.Forms.Button btnCustom7;
        private System.Windows.Forms.Button btnCustom8;
        private System.Windows.Forms.Button btnCustom9;
        private System.Windows.Forms.Button btnCustom10;
        private System.Windows.Forms.Button btnCustom11;
        private System.Windows.Forms.Button btnCustom12;
        private System.Windows.Forms.Button btnCustom13;
        private System.Windows.Forms.Button btnCustom14;
        private System.Windows.Forms.Button btnCustom15;
        private System.Windows.Forms.Button btnCustom16;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label lbInfo;
        private System.Windows.Forms.ContextMenuStrip cmsFCTB;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeSelect;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeCopy;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeSendLine;
        private System.Windows.Forms.ToolStripMenuItem cmsCodePaste;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TableLayoutPanel tLPLinks;
        private System.Windows.Forms.TableLayoutPanel tLPRechts;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsUnten;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsOben;
        private FastColoredTextBoxNS.FastColoredTextBox fCTBCode;
        private System.Windows.Forms.TableLayoutPanel tLPCustomButton2;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tLPMitteUnten;
        private System.Windows.Forms.TextBox tBURL;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Button btnStreamCheck;
        private System.Windows.Forms.Button btnStreamStop;
        private System.Windows.Forms.ContextMenuStrip cmsPictureBox;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMarkFirstPos;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxDeletePath;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem deletenotMarkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem machineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlStreamingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem createGCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem textWizzardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createSimpleShapesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMachineParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMachineParametersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem control2ndGRBLToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deutschToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gCodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mirrorYToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem rotate90ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate90ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rotateFreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_rotate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem sToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_XY_scale;
        private System.Windows.Forms.ToolStripMenuItem skalierenXYToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_XY_X_scale;
        private System.Windows.Forms.ToolStripMenuItem skalierenXYUmXUnitsZuErreichenToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_XY_Y_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereXUmToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_X_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereAufXUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_X_X_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereXAufDrehachseToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_X_A_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereYUmToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_Y_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereAufYUnitsToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_Y_Y_scale;
        private System.Windows.Forms.ToolStripMenuItem skaliereYAufDrehachseToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_Y_A_scale;
        private System.Windows.Forms.TextBox lbDimension;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rBOrigin9;
        private System.Windows.Forms.RadioButton rBOrigin8;
        private System.Windows.Forms.RadioButton rBOrigin7;
        private System.Windows.Forms.RadioButton rBOrigin6;
        private System.Windows.Forms.RadioButton rBOrigin5;
        private System.Windows.Forms.RadioButton rBOrigin4;
        private System.Windows.Forms.RadioButton rBOrigin3;
        private System.Windows.Forms.RadioButton rBOrigin2;
        private System.Windows.Forms.RadioButton rBOrigin1;
        private System.Windows.Forms.Button btnOffsetApply;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tbOffsetY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbOffsetX;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem rotaryDimaeterToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_rotary_diameter;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem ersetzteG23DurchLinienToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxResetZooming;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.Label label_a;
        private System.Windows.Forms.Button btnZeroA;
        private System.Windows.Forms.Label label_ma;
        private System.Windows.Forms.Label label_wa;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxPasteFromClipboard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.GroupBox gBOverrideFRGB;
        private System.Windows.Forms.Button btnOverrideFR1;
        private System.Windows.Forms.Button btnOverrideFR2;
        private System.Windows.Forms.Button btnOverrideFR0;
        private System.Windows.Forms.Button btnOverrideFR4;
        private System.Windows.Forms.Button btnOverrideFR3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblOverrideFRValue;
        private System.Windows.Forms.GroupBox gBOverrideSSGB;
        private System.Windows.Forms.Button btnOverrideSS2;
        private System.Windows.Forms.Button btnOverrideSS0;
        private System.Windows.Forms.Button btnOverrideSS1;
        private System.Windows.Forms.Button btnOverrideSS4;
        private System.Windows.Forms.Button btnOverrideSS3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblOverrideSSValue;
        private System.Windows.Forms.ToolStripMenuItem cmsCommentOut;
        private System.Windows.Forms.Timer gamePadTimer;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMoveToMarkedPosition;
        private System.Windows.Forms.Label lblCurrentG;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxReloadFile;
        private System.Windows.Forms.Button btnLimitExceed;
        private System.Windows.Forms.ToolStripMenuItem startStreamingAtLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_StreamLine;
        private System.Windows.Forms.Label lblStatusSpeed;
        private System.Windows.Forms.Label lblStatusFeed;
        private System.Windows.Forms.ToolStripMenuItem coordinateSystemsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewMachine;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewTool;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxSetGCodeAsBackground;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxClearBackground;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewBackground;
        private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewMachineFix;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxZeroXYAtMarkedPosition;
        private System.Windows.Forms.ToolStripMenuItem removeAnyZMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsUpdate2DView;
        private System.Windows.Forms.ToolStripMenuItem cmsEditorHotkeys;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem cmsReplaceDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        private System.Windows.Forms.ToolStripMenuItem cmsFindDialog;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsUntenRechts;
        private System.Windows.Forms.GroupBox gB_Jogging;
        private System.Windows.Forms.CheckBox cBSendJogStop;
        private System.Windows.Forms.Button btnResume;
        private virtualJoystick.virtualJoystick virtualJoystickA;
        private System.Windows.Forms.Button btnJogStop;
        private System.Windows.Forms.GroupBox gB_Jog0;
        private System.Windows.Forms.Button btnJogZeroA;
        private System.Windows.Forms.Button btnJogZeroX;
        private System.Windows.Forms.Button btnJogZeroXY;
        private System.Windows.Forms.Button btnJogZeroY;
        private System.Windows.Forms.Button btnJogZeroZ;
        private System.Windows.Forms.Label lblTool;
        private System.Windows.Forms.CheckBox cBTool;
        private virtualJoystick.virtualJoystick virtualJoystickZ;
        private virtualJoystick.virtualJoystick virtualJoystickXY;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.TextBox tBSpeed;
        private System.Windows.Forms.CheckBox cBCoolant;
        private System.Windows.Forms.CheckBox cBSpindle;
        private System.Windows.Forms.Button btnKillAlarm;
        private System.Windows.Forms.Button btnFeedHold;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.TableLayoutPanel tLPRechtsUntenRechtsMitte;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.ToolStripMenuItem mirrorRotaryToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label_c;
        private System.Windows.Forms.Button btnZeroC;
        private System.Windows.Forms.Label label_mc;
        private System.Windows.Forms.Label label_wc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnZeroB;
        private System.Windows.Forms.Label label_mb;
        private System.Windows.Forms.Label label_wb;
        private virtualJoystick.virtualJoystick virtualJoystickB;
        private virtualJoystick.virtualJoystick virtualJoystickC;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem convertZToSspindleSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate180ToolStripMenuItem;
        private System.Windows.Forms.GroupBox gBoxDimension;
        private System.Windows.Forms.GroupBox gBoxOverride;
        private System.Windows.Forms.Button btnOverrideSpindle;
        private System.Windows.Forms.Button btnOverrideMist;
        private System.Windows.Forms.Button btnOverrideFlood;
        private System.Windows.Forms.GroupBox gBOverrideRGB;
        private System.Windows.Forms.GroupBox gBOverrideASGB;
        private System.Windows.Forms.Button btnOverrideDoor;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnOverrideRapid0;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnOverrideRapid2;
        private System.Windows.Forms.Label lblOverrideRapidValue;
        private System.Windows.Forms.Button btnOverrideRapid1;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxCropSelectedPath;
        private System.Windows.Forms.ToolStripMenuItem unDoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem unDo2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem toolStrip_RadiusComp;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tBRadiusCompValue;
        private System.Windows.Forms.ToolStripMenuItem laserToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsCodePasteSpecial1;
        private System.Windows.Forms.ToolStripMenuItem cmsCodePasteSpecial2;
        private System.Windows.Forms.ToolStripMenuItem pусскийToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem franzToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chinesischToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tLPCustomButton1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem edgeFinderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem workpieceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem probingToolLengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem surfaceScanHeightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewPenUp;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMoveSelectedPathInCode;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewRuler;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator19;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksFold;
        private System.Windows.Forms.ToolStripMenuItem foldCodeBlocks1stLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldCodeBlocks2ndLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem foldCodeBlocks3rdLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandCodeBlocksToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewInfo;
        private System.Windows.Forms.ToolStripMenuItem portuguêsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem arabischToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem japanischToolStripMenuItem;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.Timer simulationTimer;
        private System.Windows.Forms.Button btnSimulateSlower;
        private System.Windows.Forms.Button btnSimulateFaster;
        private System.Windows.Forms.Button btnSimulatePause;
        private System.Windows.Forms.CheckBox cBServoButtons;
        private System.Windows.Forms.Button btnPenDown;
        private System.Windows.Forms.Button btnPenUp;
        private System.Windows.Forms.CheckBox cBMoveG0;
        private System.Windows.Forms.ToolStripMenuItem startExtensionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripViewDimension;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel0;
        private System.Windows.Forms.ToolStripMenuItem cmsEditMode;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMoveGraphicsOrigin;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksMove;
        private System.Windows.Forms.ToolStripMenuItem cmsFCTBMoveSelectedCodeBlockMostUp;
        private System.Windows.Forms.ToolStripMenuItem cmsFCTBMoveSelectedCodeBlockUp;
        private System.Windows.Forms.ToolStripMenuItem cmsFCTBMoveSelectedCodeBlockDown;
        private System.Windows.Forms.ToolStripMenuItem cmsFCTBMoveSelectedCodeBlockMostDown;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksRemoveAll;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSort;
        private System.Windows.Forms.ToolStripMenuItem unDo3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksRemoveGroup;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortById;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortReverse;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByGeometry;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByToolNr;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByToolName;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByColor;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByCodeSize;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByCodeArea;
        private System.Windows.Forms.Timer SplashScreenTimer;
        private System.Windows.Forms.ToolStripMenuItem cmsCodeBlocksSortByDistance;
        private System.Windows.Forms.ToolStripMenuItem copyContentTroClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createBarcodeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxReverseSelectedPath;
        private System.Windows.Forms.ToolStripMenuItem sortByLayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByPenWidthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortByTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Timer loadTimer;
        private System.Windows.Forms.ToolStripMenuItem jogPathCreatorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createJogPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem control3rdGRBLToolStripMenuItem;
    }
}

