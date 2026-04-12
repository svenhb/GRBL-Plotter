/*  GRBL-Plotter. Another GCode sender for GRBL.
    This file is part of the GRBL-Plotter application.
   
    Copyright (C) 2015-2026 Sven Hasemann contact: svenhb@web.de

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
/*
 * 2026-01-05 in .designer set	this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
								this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
*/
namespace GrblPlotter
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
                if (picBoxBackround != null)
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
				penGrid1.Dispose();
				penGrid10.Dispose();
                penGrid100.Dispose();
                penTool.Dispose();
                penMarker.Dispose();
                penLandMark.Dispose();
				penBackgroundPath.Dispose();
				penDimension.Dispose();
                penSimulation.Dispose();

                brushMachineLimit.Dispose();
                brushBackground.Dispose();
				brushBackgroundPath.Dispose();
                StyleAAxis.Dispose();
                StyleLineN.Dispose();
                pBoxOrig.Dispose();
                pBoxTransform.Dispose();

                ErrorStyle.Dispose();
                StyleTT.Dispose();
                Style2nd.Dispose();
				
			/*	_text_form.Dispose();
				_image_form.Dispose();
				_shape_form.Dispose();
				_barcode_form.Dispose();

				_streaming_form.Dispose();
				_streaming_form2.Dispose();
				_serial_form2.Dispose();
				_serial_form3.Dispose();
				_2ndGRBL_form.Dispose();
				_camera_form.Dispose();
				_diyControlPad.Dispose();
				_coordSystem_form.Dispose();
				_laser_form.Dispose();
				_probing_form.Dispose();
				_heightmap_form.Dispose();
				_setup_form.Dispose();
				_jogPathCreator_form.Dispose();*/
				
				myFont1.Dispose();
    			myFont2.Dispose();

                selectionPathOrig.Dispose();
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
            GrblPlotter.Properties.Settings settings1 = new GrblPlotter.Properties.Settings();
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
            this.toggleBlockExpansionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.cmsPictureBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.unDo2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.applyLastTransformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxReloadFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxReloadFile2 = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxPasteFromClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxClearWorkspace = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxResetZooming = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxMoveToMarkedPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxZeroXYAtMarkedPosition = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxMoveGraphicsOrigin = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxMoveGraphicsOriginTo00 = new System.Windows.Forms.ToolStripMenuItem();
            this.deletenotMarkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxMarkFirstPos = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxShowProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxDuplicatePath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxDeletePath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxCropSelectedPath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxMoveSelectedPathInCode = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxReverseSelectedPath = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxRotateSelectedPath = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsPicBoxSetGCodeAsBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPicBoxClearBackground = new System.Windows.Forms.ToolStripMenuItem();
            this.copyContentTroClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.LanguageToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deutschToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pусскийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.portuguêsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.franzToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.italianoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PolishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.czechToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.türkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chinesischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.arabischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.japanischToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createGCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textWizzardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createBarcodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createSimpleShapesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.directControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireCutterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createJogPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startExtensionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unDoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useOrigin = new System.Windows.Forms.ToolStripMenuItem();
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
            this.convertToPolarCoordinatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.projectorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grblSetupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.processAutomationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.showFormsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MainTimer = new System.Windows.Forms.Timer(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.gamePadTimer = new System.Windows.Forms.Timer(this.components);
            this.simulationTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel0 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.SplashScreenTimer = new System.Windows.Forms.Timer(this.components);
            this.loadTimer = new System.Windows.Forms.Timer(this.components);
            this.cmsPictureBox2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cms2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.moveBetweenLastPositionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveTimer = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tLPLinks = new System.Windows.Forms.TableLayoutPanel();
            this.ucStreaming = new GrblPlotter.UserControls.UCStreaming();
            this.ucOverrides = new GrblPlotter.UserControls.UCOverrides();
            this.fCTBCode = new FastColoredTextBoxNS.FastColoredTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tBURL = new System.Windows.Forms.TextBox();
            this.Rb2DViewMode1 = new System.Windows.Forms.RadioButton();
            this.Rb2DViewMode2 = new System.Windows.Forms.RadioButton();
            this.Rb2DViewMode3 = new System.Windows.Forms.RadioButton();
            this.CbAddGraphic = new System.Windows.Forms.CheckBox();
            this.ucFlowControl = new GrblPlotter.UserControls.UCFlowControl();
            this.ucSetOffset = new GrblPlotter.UserControls.UCSetOffset();
            this.tLPRechts = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUnten = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsUntenRechts = new System.Windows.Forms.TableLayoutPanel();
            this.ucMoveToGraphic = new GrblPlotter.UserControls.UCMoveToGraphic();
            this.ucMoveToZero = new GrblPlotter.UserControls.UCMoveToZero();
            this.ucJogControlAll = new GrblPlotter.UserControls.UCJogControlAll();
            this.tC_RouterPlotterLaser2 = new System.Windows.Forms.TabControl();
            this.tabPageLaser = new System.Windows.Forms.TabPage();
            this.ucDeviceLaser2 = new GrblPlotter.UserControls.UCDeviceLaser2();
            this.tabPagePlotter = new System.Windows.Forms.TabPage();
            this.ucDevicePlotter2 = new GrblPlotter.UserControls.UCDevicePlotter2();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.ucDeviceRouter2 = new GrblPlotter.UserControls.UCDeviceRouter2();
            this.ucSetCoordinateSystem = new GrblPlotter.UserControls.UCSetCoordinateSystem();
            this.tLPMitteUnten = new System.Windows.Forms.TableLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tLPMitteUnten1Zeile = new System.Windows.Forms.TableLayoutPanel();
            this.tLPRechtsOben = new System.Windows.Forms.TableLayoutPanel();
            this.ucdro = new GrblPlotter.UserControls.UCDRO();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.GbCustomButtons = new System.Windows.Forms.GroupBox();
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
            this.ucToolList = new GrblPlotter.UserControls.UCToolList();
            this.tC_RouterPlotterLaser = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.ucDeviceLaser = new GrblPlotter.UserControls.UCDeviceLaser();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.ucDevicePlotter = new GrblPlotter.UserControls.UCDevicePlotter();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ucDeviceRouter = new GrblPlotter.UserControls.UCDeviceRouter();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.IndividualSettings = new System.Windows.Forms.Label();
            this.cmsFCTB.SuspendLayout();
            this.cmsPictureBox.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.cmsPictureBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tLPLinks.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).BeginInit();
            this.tLPRechts.SuspendLayout();
            this.tLPRechtsUnten.SuspendLayout();
            this.tLPRechtsUntenRechts.SuspendLayout();
            this.tC_RouterPlotterLaser2.SuspendLayout();
            this.tabPageLaser.SuspendLayout();
            this.tabPagePlotter.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tLPMitteUnten.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tLPMitteUnten1Zeile.SuspendLayout();
            this.tLPRechtsOben.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.GbCustomButtons.SuspendLayout();
            this.tLPCustomButton1.SuspendLayout();
            this.tLPCustomButton2.SuspendLayout();
            this.tC_RouterPlotterLaser.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmsFCTB
            // 
            this.cmsFCTB.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            this.cmsFCTB.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.CmsFctb_ItemClicked);
            // 
            // unDo3ToolStripMenuItem
            // 
            this.unDo3ToolStripMenuItem.Name = "unDo3ToolStripMenuItem";
            resources.ApplyResources(this.unDo3ToolStripMenuItem, "unDo3ToolStripMenuItem");
            this.unDo3ToolStripMenuItem.Click += new System.EventHandler(this.UnDoToolStripMenuItem_Click);
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
            this.expandCodeBlocksToolStripMenuItem1,
            this.toggleBlockExpansionToolStripMenuItem});
            this.cmsCodeBlocksFold.Name = "cmsCodeBlocksFold";
            // 
            // foldCodeBlocks1stLevelToolStripMenuItem
            // 
            this.foldCodeBlocks1stLevelToolStripMenuItem.Name = "foldCodeBlocks1stLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks1stLevelToolStripMenuItem, "foldCodeBlocks1stLevelToolStripMenuItem");
            this.foldCodeBlocks1stLevelToolStripMenuItem.Click += new System.EventHandler(this.FoldBlocks1stToolStripMenuItem1_Click);
            // 
            // foldCodeBlocks2ndLevelToolStripMenuItem
            // 
            this.foldCodeBlocks2ndLevelToolStripMenuItem.Name = "foldCodeBlocks2ndLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks2ndLevelToolStripMenuItem, "foldCodeBlocks2ndLevelToolStripMenuItem");
            this.foldCodeBlocks2ndLevelToolStripMenuItem.Click += new System.EventHandler(this.FoldBlocks2ndToolStripMenuItem1_Click);
            // 
            // foldCodeBlocks3rdLevelToolStripMenuItem
            // 
            this.foldCodeBlocks3rdLevelToolStripMenuItem.Name = "foldCodeBlocks3rdLevelToolStripMenuItem";
            resources.ApplyResources(this.foldCodeBlocks3rdLevelToolStripMenuItem, "foldCodeBlocks3rdLevelToolStripMenuItem");
            this.foldCodeBlocks3rdLevelToolStripMenuItem.Click += new System.EventHandler(this.FoldBlocks3rdToolStripMenuItem1_Click);
            // 
            // expandCodeBlocksToolStripMenuItem1
            // 
            this.expandCodeBlocksToolStripMenuItem1.Name = "expandCodeBlocksToolStripMenuItem1";
            resources.ApplyResources(this.expandCodeBlocksToolStripMenuItem1, "expandCodeBlocksToolStripMenuItem1");
            this.expandCodeBlocksToolStripMenuItem1.Click += new System.EventHandler(this.ExpandCodeBlocksToolStripMenuItem_Click);
            // 
            // toggleBlockExpansionToolStripMenuItem
            // 
            this.toggleBlockExpansionToolStripMenuItem.Checked = true;
            this.toggleBlockExpansionToolStripMenuItem.CheckOnClick = true;
            this.toggleBlockExpansionToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toggleBlockExpansionToolStripMenuItem.Name = "toggleBlockExpansionToolStripMenuItem";
            resources.ApplyResources(this.toggleBlockExpansionToolStripMenuItem, "toggleBlockExpansionToolStripMenuItem");
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
            this.cmsFCTBMoveSelectedCodeBlockMostUp.Click += new System.EventHandler(this.MoveSelectedCodeBlockMostUpToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockUp
            // 
            this.cmsFCTBMoveSelectedCodeBlockUp.Name = "cmsFCTBMoveSelectedCodeBlockUp";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockUp, "cmsFCTBMoveSelectedCodeBlockUp");
            this.cmsFCTBMoveSelectedCodeBlockUp.Click += new System.EventHandler(this.MoveSelectedCodeBlockUpToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockDown
            // 
            this.cmsFCTBMoveSelectedCodeBlockDown.Name = "cmsFCTBMoveSelectedCodeBlockDown";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockDown, "cmsFCTBMoveSelectedCodeBlockDown");
            this.cmsFCTBMoveSelectedCodeBlockDown.Click += new System.EventHandler(this.MoveSelectedCodeBlockDownToolStripMenuItem_Click);
            // 
            // cmsFCTBMoveSelectedCodeBlockMostDown
            // 
            this.cmsFCTBMoveSelectedCodeBlockMostDown.Name = "cmsFCTBMoveSelectedCodeBlockMostDown";
            resources.ApplyResources(this.cmsFCTBMoveSelectedCodeBlockMostDown, "cmsFCTBMoveSelectedCodeBlockMostDown");
            this.cmsFCTBMoveSelectedCodeBlockMostDown.Click += new System.EventHandler(this.MoveSelectedCodeBlockMostDownToolStripMenuItem_Click);
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
            this.cmsCodeBlocksSortById.Click += new System.EventHandler(this.CmsCodeBlocksSortById_Click);
            // 
            // cmsCodeBlocksSortByColor
            // 
            this.cmsCodeBlocksSortByColor.Name = "cmsCodeBlocksSortByColor";
            resources.ApplyResources(this.cmsCodeBlocksSortByColor, "cmsCodeBlocksSortByColor");
            this.cmsCodeBlocksSortByColor.Click += new System.EventHandler(this.CmsCodeBlocksSortByColor_Click);
            // 
            // sortByPenWidthToolStripMenuItem
            // 
            this.sortByPenWidthToolStripMenuItem.Name = "sortByPenWidthToolStripMenuItem";
            resources.ApplyResources(this.sortByPenWidthToolStripMenuItem, "sortByPenWidthToolStripMenuItem");
            this.sortByPenWidthToolStripMenuItem.Click += new System.EventHandler(this.CmsCodeBlocksSortByWidth_Click);
            // 
            // sortByLayerToolStripMenuItem
            // 
            this.sortByLayerToolStripMenuItem.Name = "sortByLayerToolStripMenuItem";
            resources.ApplyResources(this.sortByLayerToolStripMenuItem, "sortByLayerToolStripMenuItem");
            this.sortByLayerToolStripMenuItem.Click += new System.EventHandler(this.CmsCodeBlocksSortByLayer_Click);
            // 
            // sortByTypeToolStripMenuItem
            // 
            this.sortByTypeToolStripMenuItem.Name = "sortByTypeToolStripMenuItem";
            resources.ApplyResources(this.sortByTypeToolStripMenuItem, "sortByTypeToolStripMenuItem");
            this.sortByTypeToolStripMenuItem.Click += new System.EventHandler(this.CmsCodeBlocksSortByType_Click);
            // 
            // cmsCodeBlocksSortByGeometry
            // 
            this.cmsCodeBlocksSortByGeometry.Name = "cmsCodeBlocksSortByGeometry";
            resources.ApplyResources(this.cmsCodeBlocksSortByGeometry, "cmsCodeBlocksSortByGeometry");
            this.cmsCodeBlocksSortByGeometry.Click += new System.EventHandler(this.CmsCodeBlocksSortByGeometry_Click);
            // 
            // cmsCodeBlocksSortByToolNr
            // 
            this.cmsCodeBlocksSortByToolNr.Name = "cmsCodeBlocksSortByToolNr";
            resources.ApplyResources(this.cmsCodeBlocksSortByToolNr, "cmsCodeBlocksSortByToolNr");
            this.cmsCodeBlocksSortByToolNr.Click += new System.EventHandler(this.CmsCodeBlocksSortByToolNr_Click);
            // 
            // cmsCodeBlocksSortByToolName
            // 
            this.cmsCodeBlocksSortByToolName.Name = "cmsCodeBlocksSortByToolName";
            resources.ApplyResources(this.cmsCodeBlocksSortByToolName, "cmsCodeBlocksSortByToolName");
            this.cmsCodeBlocksSortByToolName.Click += new System.EventHandler(this.CmsCodeBlocksSortByToolName_Click);
            // 
            // cmsCodeBlocksSortByCodeSize
            // 
            this.cmsCodeBlocksSortByCodeSize.Name = "cmsCodeBlocksSortByCodeSize";
            resources.ApplyResources(this.cmsCodeBlocksSortByCodeSize, "cmsCodeBlocksSortByCodeSize");
            this.cmsCodeBlocksSortByCodeSize.Click += new System.EventHandler(this.CmsCodeBlocksSortByCodeSize_Click);
            // 
            // cmsCodeBlocksSortByCodeArea
            // 
            this.cmsCodeBlocksSortByCodeArea.Name = "cmsCodeBlocksSortByCodeArea";
            resources.ApplyResources(this.cmsCodeBlocksSortByCodeArea, "cmsCodeBlocksSortByCodeArea");
            this.cmsCodeBlocksSortByCodeArea.Click += new System.EventHandler(this.CmsCodeBlocksSortByCodeArea_Click);
            // 
            // cmsCodeBlocksSortByDistance
            // 
            this.cmsCodeBlocksSortByDistance.Name = "cmsCodeBlocksSortByDistance";
            resources.ApplyResources(this.cmsCodeBlocksSortByDistance, "cmsCodeBlocksSortByDistance");
            this.cmsCodeBlocksSortByDistance.Click += new System.EventHandler(this.CmsCodeBlocksSortByDistance_Click);
            // 
            // cmsCodeBlocksRemoveGroup
            // 
            resources.ApplyResources(this.cmsCodeBlocksRemoveGroup, "cmsCodeBlocksRemoveGroup");
            this.cmsCodeBlocksRemoveGroup.Name = "cmsCodeBlocksRemoveGroup";
            this.cmsCodeBlocksRemoveGroup.Click += new System.EventHandler(this.CmsCodeBlocksRemoveGroup_Click);
            // 
            // cmsCodeBlocksRemoveAll
            // 
            resources.ApplyResources(this.cmsCodeBlocksRemoveAll, "cmsCodeBlocksRemoveAll");
            this.cmsCodeBlocksRemoveAll.Name = "cmsCodeBlocksRemoveAll";
            this.cmsCodeBlocksRemoveAll.Click += new System.EventHandler(this.CmsCodeBlocksRemoveAll_Click);
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
            // cmsPictureBox
            // 
            this.cmsPictureBox.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsPictureBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unDo2ToolStripMenuItem,
            this.applyLastTransformToolStripMenuItem,
            this.toolStripSeparator17,
            this.cmsPicBoxReloadFile,
            this.cmsPicBoxReloadFile2,
            this.cmsPicBoxPasteFromClipboard,
            this.cmsPicBoxClearWorkspace,
            this.toolStripSeparator9,
            this.cmsPicBoxResetZooming,
            this.toolStripSeparator8,
            this.cmsPicBoxMoveToMarkedPosition,
            this.cmsPicBoxZeroXYAtMarkedPosition,
            this.cmsPicBoxMoveGraphicsOrigin,
            this.cmsPicBoxMoveGraphicsOriginTo00,
            this.deletenotMarkToolStripMenuItem,
            this.toolStripSeparator1,
            this.cmsPicBoxMarkFirstPos,
            this.cmsPicBoxShowProperties,
            this.cmsPicBoxDuplicatePath,
            this.cmsPicBoxDeletePath,
            this.cmsPicBoxCropSelectedPath,
            this.cmsPicBoxMoveSelectedPathInCode,
            this.cmsPicBoxReverseSelectedPath,
            this.cmsPicBoxRotateSelectedPath,
            this.toolStripSeparator10,
            this.cmsPicBoxSetGCodeAsBackground,
            this.cmsPicBoxClearBackground,
            this.copyContentTroClipboardToolStripMenuItem});
            this.cmsPictureBox.Name = "cmsPictureBox";
            this.cmsPictureBox.ShowImageMargin = false;
            resources.ApplyResources(this.cmsPictureBox, "cmsPictureBox");
            this.cmsPictureBox.Opening += new System.ComponentModel.CancelEventHandler(this.CmsPictureBox_Opening);
            // 
            // unDo2ToolStripMenuItem
            // 
            resources.ApplyResources(this.unDo2ToolStripMenuItem, "unDo2ToolStripMenuItem");
            this.unDo2ToolStripMenuItem.Name = "unDo2ToolStripMenuItem";
            this.unDo2ToolStripMenuItem.Click += new System.EventHandler(this.UnDoToolStripMenuItem_Click);
            // 
            // applyLastTransformToolStripMenuItem
            // 
            this.applyLastTransformToolStripMenuItem.Name = "applyLastTransformToolStripMenuItem";
            resources.ApplyResources(this.applyLastTransformToolStripMenuItem, "applyLastTransformToolStripMenuItem");
            this.applyLastTransformToolStripMenuItem.Click += new System.EventHandler(this.ApplyLastTransformToolStripMenuItem_Click);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // cmsPicBoxReloadFile
            // 
            this.cmsPicBoxReloadFile.Name = "cmsPicBoxReloadFile";
            resources.ApplyResources(this.cmsPicBoxReloadFile, "cmsPicBoxReloadFile");
            this.cmsPicBoxReloadFile.Tag = "";
            this.cmsPicBoxReloadFile.Click += new System.EventHandler(this.CmsPicBoxReloadFile_Click);
            // 
            // cmsPicBoxReloadFile2
            // 
            this.cmsPicBoxReloadFile2.Name = "cmsPicBoxReloadFile2";
            resources.ApplyResources(this.cmsPicBoxReloadFile2, "cmsPicBoxReloadFile2");
            this.cmsPicBoxReloadFile2.Tag = "";
            this.cmsPicBoxReloadFile2.Click += new System.EventHandler(this.CmsPicBoxReloadFile_Click);
            // 
            // cmsPicBoxPasteFromClipboard
            // 
            this.cmsPicBoxPasteFromClipboard.Name = "cmsPicBoxPasteFromClipboard";
            resources.ApplyResources(this.cmsPicBoxPasteFromClipboard, "cmsPicBoxPasteFromClipboard");
            this.cmsPicBoxPasteFromClipboard.Click += new System.EventHandler(this.CmsPicBoxPasteFromClipboard_Click);
            // 
            // cmsPicBoxClearWorkspace
            // 
            this.cmsPicBoxClearWorkspace.Name = "cmsPicBoxClearWorkspace";
            resources.ApplyResources(this.cmsPicBoxClearWorkspace, "cmsPicBoxClearWorkspace");
            this.cmsPicBoxClearWorkspace.Click += new System.EventHandler(this.CmsPicBoxClearWorkspace_Click);
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
            this.cmsPicBoxResetZooming.Click += new System.EventHandler(this.CmsPicBoxResetZooming_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // cmsPicBoxMoveToMarkedPosition
            // 
            this.cmsPicBoxMoveToMarkedPosition.Name = "cmsPicBoxMoveToMarkedPosition";
            resources.ApplyResources(this.cmsPicBoxMoveToMarkedPosition, "cmsPicBoxMoveToMarkedPosition");
            this.cmsPicBoxMoveToMarkedPosition.Click += new System.EventHandler(this.CmsPicBoxMoveToMarkedPosition_Click);
            // 
            // cmsPicBoxZeroXYAtMarkedPosition
            // 
            this.cmsPicBoxZeroXYAtMarkedPosition.Name = "cmsPicBoxZeroXYAtMarkedPosition";
            resources.ApplyResources(this.cmsPicBoxZeroXYAtMarkedPosition, "cmsPicBoxZeroXYAtMarkedPosition");
            this.cmsPicBoxZeroXYAtMarkedPosition.Click += new System.EventHandler(this.CmsPicBoxZeroXYAtMarkedPosition_Click);
            // 
            // cmsPicBoxMoveGraphicsOrigin
            // 
            this.cmsPicBoxMoveGraphicsOrigin.Name = "cmsPicBoxMoveGraphicsOrigin";
            resources.ApplyResources(this.cmsPicBoxMoveGraphicsOrigin, "cmsPicBoxMoveGraphicsOrigin");
            this.cmsPicBoxMoveGraphicsOrigin.Click += new System.EventHandler(this.CmsPicBoxMoveGraphicsOrigin_Click);
            // 
            // cmsPicBoxMoveGraphicsOriginTo00
            // 
            this.cmsPicBoxMoveGraphicsOriginTo00.Name = "cmsPicBoxMoveGraphicsOriginTo00";
            resources.ApplyResources(this.cmsPicBoxMoveGraphicsOriginTo00, "cmsPicBoxMoveGraphicsOriginTo00");
            this.cmsPicBoxMoveGraphicsOriginTo00.Click += new System.EventHandler(this.CmsPicBoxMoveGraphicsOrigin2_Click);
            // 
            // deletenotMarkToolStripMenuItem
            // 
            resources.ApplyResources(this.deletenotMarkToolStripMenuItem, "deletenotMarkToolStripMenuItem");
            this.deletenotMarkToolStripMenuItem.Name = "deletenotMarkToolStripMenuItem";
            this.deletenotMarkToolStripMenuItem.Click += new System.EventHandler(this.DeletenotMarkToolStripMenuItem_Click);
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
            this.cmsPicBoxMarkFirstPos.Click += new System.EventHandler(this.CmsPicBoxMoveToFirstPos_Click);
            // 
            // cmsPicBoxShowProperties
            // 
            resources.ApplyResources(this.cmsPicBoxShowProperties, "cmsPicBoxShowProperties");
            this.cmsPicBoxShowProperties.Name = "cmsPicBoxShowProperties";
            this.cmsPicBoxShowProperties.Click += new System.EventHandler(this.CmsPicBoxShowProperties_Click);
            // 
            // cmsPicBoxDuplicatePath
            // 
            resources.ApplyResources(this.cmsPicBoxDuplicatePath, "cmsPicBoxDuplicatePath");
            this.cmsPicBoxDuplicatePath.Name = "cmsPicBoxDuplicatePath";
            this.cmsPicBoxDuplicatePath.Click += new System.EventHandler(this.CmsPicBoxDuplicatePath_Click);
            // 
            // cmsPicBoxDeletePath
            // 
            resources.ApplyResources(this.cmsPicBoxDeletePath, "cmsPicBoxDeletePath");
            this.cmsPicBoxDeletePath.Name = "cmsPicBoxDeletePath";
            this.cmsPicBoxDeletePath.Click += new System.EventHandler(this.CmsPicBoxDeletePath_Click);
            // 
            // cmsPicBoxCropSelectedPath
            // 
            resources.ApplyResources(this.cmsPicBoxCropSelectedPath, "cmsPicBoxCropSelectedPath");
            this.cmsPicBoxCropSelectedPath.Name = "cmsPicBoxCropSelectedPath";
            this.cmsPicBoxCropSelectedPath.Click += new System.EventHandler(this.CmsPicBoxCropSelectedPath_Click);
            // 
            // cmsPicBoxMoveSelectedPathInCode
            // 
            resources.ApplyResources(this.cmsPicBoxMoveSelectedPathInCode, "cmsPicBoxMoveSelectedPathInCode");
            this.cmsPicBoxMoveSelectedPathInCode.Name = "cmsPicBoxMoveSelectedPathInCode";
            this.cmsPicBoxMoveSelectedPathInCode.Click += new System.EventHandler(this.CmsPicBoxMoveSelectedPathInCode_Click);
            // 
            // cmsPicBoxReverseSelectedPath
            // 
            resources.ApplyResources(this.cmsPicBoxReverseSelectedPath, "cmsPicBoxReverseSelectedPath");
            this.cmsPicBoxReverseSelectedPath.Name = "cmsPicBoxReverseSelectedPath";
            this.cmsPicBoxReverseSelectedPath.Click += new System.EventHandler(this.CmsPicBoxReverseSelectedPath_Click);
            // 
            // cmsPicBoxRotateSelectedPath
            // 
            resources.ApplyResources(this.cmsPicBoxRotateSelectedPath, "cmsPicBoxRotateSelectedPath");
            this.cmsPicBoxRotateSelectedPath.Name = "cmsPicBoxRotateSelectedPath";
            this.cmsPicBoxRotateSelectedPath.Click += new System.EventHandler(this.CmsPicBoxRotateSelectedPath_Click);
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
            this.cmsPicBoxSetGCodeAsBackground.Click += new System.EventHandler(this.CmsPicBoxSetGCodeAsBackground_Click);
            // 
            // cmsPicBoxClearBackground
            // 
            this.cmsPicBoxClearBackground.Name = "cmsPicBoxClearBackground";
            resources.ApplyResources(this.cmsPicBoxClearBackground, "cmsPicBoxClearBackground");
            this.cmsPicBoxClearBackground.Click += new System.EventHandler(this.CmsPicBoxClearBackground_Click);
            // 
            // copyContentTroClipboardToolStripMenuItem
            // 
            this.copyContentTroClipboardToolStripMenuItem.Name = "copyContentTroClipboardToolStripMenuItem";
            resources.ApplyResources(this.copyContentTroClipboardToolStripMenuItem, "copyContentTroClipboardToolStripMenuItem");
            this.copyContentTroClipboardToolStripMenuItem.Click += new System.EventHandler(this.CopyContentToClipboardToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.createGCodeToolStripMenuItem,
            this.gCodeToolStripMenuItem,
            this.workpieceToolStripMenuItem,
            this.machineToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.logToolStripMenuItem,
            this.showFormsToolStripMenuItem});
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
            this.LanguageToolStripMenuItem3,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            resources.ApplyResources(this.loadToolStripMenuItem, "loadToolStripMenuItem");
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.BtnOpenFile_Click);
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
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.BtnSaveFile_Click);
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
            this.setupToolStripMenuItem1.Click += new System.EventHandler(this.SetupToolStripMenuItem_Click);
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
            this.saveMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.SaveMachineParametersToolStripMenuItem_Click);
            // 
            // loadMachineParametersToolStripMenuItem
            // 
            this.loadMachineParametersToolStripMenuItem.Name = "loadMachineParametersToolStripMenuItem";
            resources.ApplyResources(this.loadMachineParametersToolStripMenuItem, "loadMachineParametersToolStripMenuItem");
            this.loadMachineParametersToolStripMenuItem.Click += new System.EventHandler(this.LoadMachineParametersToolStripMenuItem_Click);
            // 
            // LanguageToolStripMenuItem3
            // 
            this.LanguageToolStripMenuItem3.AutoToolTip = true;
            this.LanguageToolStripMenuItem3.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.deutschToolStripMenuItem,
            this.pусскийToolStripMenuItem,
            this.toolStripMenuItem4,
            this.portuguêsToolStripMenuItem,
            this.franzToolStripMenuItem,
            this.italianoToolStripMenuItem,
            this.PolishToolStripMenuItem,
            this.czechToolStripMenuItem,
            this.türkToolStripMenuItem,
            this.chinesischToolStripMenuItem,
            this.arabischToolStripMenuItem,
            this.japanischToolStripMenuItem});
            this.LanguageToolStripMenuItem3.Name = "LanguageToolStripMenuItem3";
            resources.ApplyResources(this.LanguageToolStripMenuItem3, "LanguageToolStripMenuItem3");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.EnglishToolStripMenuItem_Click);
            // 
            // deutschToolStripMenuItem
            // 
            this.deutschToolStripMenuItem.Name = "deutschToolStripMenuItem";
            resources.ApplyResources(this.deutschToolStripMenuItem, "deutschToolStripMenuItem");
            this.deutschToolStripMenuItem.Click += new System.EventHandler(this.DeutschToolStripMenuItem_Click);
            // 
            // pусскийToolStripMenuItem
            // 
            this.pусскийToolStripMenuItem.Name = "pусскийToolStripMenuItem";
            resources.ApplyResources(this.pусскийToolStripMenuItem, "pусскийToolStripMenuItem");
            this.pусскийToolStripMenuItem.Click += new System.EventHandler(this.RussianToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Click += new System.EventHandler(this.SpanToolStripMenuItem_Click);
            // 
            // portuguêsToolStripMenuItem
            // 
            this.portuguêsToolStripMenuItem.Name = "portuguêsToolStripMenuItem";
            resources.ApplyResources(this.portuguêsToolStripMenuItem, "portuguêsToolStripMenuItem");
            this.portuguêsToolStripMenuItem.Click += new System.EventHandler(this.PortugisischToolStripMenuItem_Click);
            // 
            // franzToolStripMenuItem
            // 
            this.franzToolStripMenuItem.Name = "franzToolStripMenuItem";
            resources.ApplyResources(this.franzToolStripMenuItem, "franzToolStripMenuItem");
            this.franzToolStripMenuItem.Click += new System.EventHandler(this.FranzToolStripMenuItem_Click);
            // 
            // italianoToolStripMenuItem
            // 
            this.italianoToolStripMenuItem.Name = "italianoToolStripMenuItem";
            resources.ApplyResources(this.italianoToolStripMenuItem, "italianoToolStripMenuItem");
            this.italianoToolStripMenuItem.Click += new System.EventHandler(this.ItalianToolStripMenuItem_Click);
            // 
            // PolishToolStripMenuItem
            // 
            this.PolishToolStripMenuItem.Name = "PolishToolStripMenuItem";
            resources.ApplyResources(this.PolishToolStripMenuItem, "PolishToolStripMenuItem");
            this.PolishToolStripMenuItem.Click += new System.EventHandler(this.PolishToolStripMenuItem_Click);
            // 
            // czechToolStripMenuItem
            // 
            this.czechToolStripMenuItem.Name = "czechToolStripMenuItem";
            resources.ApplyResources(this.czechToolStripMenuItem, "czechToolStripMenuItem");
            this.czechToolStripMenuItem.Click += new System.EventHandler(this.CzechToolStripMenuItem_Click);
            // 
            // türkToolStripMenuItem
            // 
            this.türkToolStripMenuItem.Name = "türkToolStripMenuItem";
            resources.ApplyResources(this.türkToolStripMenuItem, "türkToolStripMenuItem");
            this.türkToolStripMenuItem.Click += new System.EventHandler(this.TürkToolStripMenuItem_Click);
            // 
            // chinesischToolStripMenuItem
            // 
            this.chinesischToolStripMenuItem.Name = "chinesischToolStripMenuItem";
            resources.ApplyResources(this.chinesischToolStripMenuItem, "chinesischToolStripMenuItem");
            this.chinesischToolStripMenuItem.Click += new System.EventHandler(this.ChinesischToolStripMenuItem_Click);
            // 
            // arabischToolStripMenuItem
            // 
            this.arabischToolStripMenuItem.Name = "arabischToolStripMenuItem";
            resources.ApplyResources(this.arabischToolStripMenuItem, "arabischToolStripMenuItem");
            this.arabischToolStripMenuItem.Click += new System.EventHandler(this.ArabischToolStripMenuItem_Click);
            // 
            // japanischToolStripMenuItem
            // 
            this.japanischToolStripMenuItem.Name = "japanischToolStripMenuItem";
            resources.ApplyResources(this.japanischToolStripMenuItem, "japanischToolStripMenuItem");
            this.japanischToolStripMenuItem.Click += new System.EventHandler(this.JapanischToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // createGCodeToolStripMenuItem
            // 
            this.createGCodeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textWizzardToolStripMenuItem,
            this.createBarcodeToolStripMenuItem,
            this.imageToolStripMenuItem,
            this.createSimpleShapesToolStripMenuItem,
            this.directControlToolStripMenuItem,
            this.wireCutterToolStripMenuItem,
            this.createJogPathToolStripMenuItem,
            this.startExtensionToolStripMenuItem});
            this.createGCodeToolStripMenuItem.Name = "createGCodeToolStripMenuItem";
            resources.ApplyResources(this.createGCodeToolStripMenuItem, "createGCodeToolStripMenuItem");
            // 
            // textWizzardToolStripMenuItem
            // 
            this.textWizzardToolStripMenuItem.Name = "textWizzardToolStripMenuItem";
            resources.ApplyResources(this.textWizzardToolStripMenuItem, "textWizzardToolStripMenuItem");
            this.textWizzardToolStripMenuItem.Click += new System.EventHandler(this.TextWizzardToolStripMenuItem_Click);
            // 
            // createBarcodeToolStripMenuItem
            // 
            this.createBarcodeToolStripMenuItem.Name = "createBarcodeToolStripMenuItem";
            resources.ApplyResources(this.createBarcodeToolStripMenuItem, "createBarcodeToolStripMenuItem");
            this.createBarcodeToolStripMenuItem.Click += new System.EventHandler(this.CreateBarcodeToolStripMenuItem_Click);
            // 
            // imageToolStripMenuItem
            // 
            this.imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            resources.ApplyResources(this.imageToolStripMenuItem, "imageToolStripMenuItem");
            this.imageToolStripMenuItem.Click += new System.EventHandler(this.ImageToolStripMenuItem_Click);
            // 
            // createSimpleShapesToolStripMenuItem
            // 
            this.createSimpleShapesToolStripMenuItem.Name = "createSimpleShapesToolStripMenuItem";
            resources.ApplyResources(this.createSimpleShapesToolStripMenuItem, "createSimpleShapesToolStripMenuItem");
            this.createSimpleShapesToolStripMenuItem.Click += new System.EventHandler(this.CreateSimpleShapesToolStripMenuItem_Click);
            // 
            // directControlToolStripMenuItem
            // 
            this.directControlToolStripMenuItem.Name = "directControlToolStripMenuItem";
            resources.ApplyResources(this.directControlToolStripMenuItem, "directControlToolStripMenuItem");
            this.directControlToolStripMenuItem.Click += new System.EventHandler(this.DirectControlToolStripMenuItem_Click);
            // 
            // wireCutterToolStripMenuItem
            // 
            this.wireCutterToolStripMenuItem.Name = "wireCutterToolStripMenuItem";
            resources.ApplyResources(this.wireCutterToolStripMenuItem, "wireCutterToolStripMenuItem");
            this.wireCutterToolStripMenuItem.Click += new System.EventHandler(this.WireCutterToolStripMenuItem_Click);
            // 
            // createJogPathToolStripMenuItem
            // 
            this.createJogPathToolStripMenuItem.Name = "createJogPathToolStripMenuItem";
            resources.ApplyResources(this.createJogPathToolStripMenuItem, "createJogPathToolStripMenuItem");
            this.createJogPathToolStripMenuItem.Click += new System.EventHandler(this.JogCreatorToolStripMenuItem_Click);
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
            this.useOrigin,
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
            this.convertToPolarCoordinatesToolStripMenuItem,
            this.convertZToSspindleSpeedToolStripMenuItem,
            this.removeAnyZMoveToolStripMenuItem});
            this.gCodeToolStripMenuItem.Name = "gCodeToolStripMenuItem";
            resources.ApplyResources(this.gCodeToolStripMenuItem, "gCodeToolStripMenuItem");
            // 
            // unDoToolStripMenuItem
            // 
            resources.ApplyResources(this.unDoToolStripMenuItem, "unDoToolStripMenuItem");
            this.unDoToolStripMenuItem.Name = "unDoToolStripMenuItem";
            this.unDoToolStripMenuItem.Click += new System.EventHandler(this.UnDoToolStripMenuItem_Click);
            // 
            // useOrigin
            // 
            this.useOrigin.CheckOnClick = true;
            this.useOrigin.Name = "useOrigin";
            resources.ApplyResources(this.useOrigin, "useOrigin");
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
            this.mirrorXToolStripMenuItem.Click += new System.EventHandler(this.MirrorXToolStripMenuItem_Click);
            // 
            // mirrorYToolStripMenuItem
            // 
            this.mirrorYToolStripMenuItem.Name = "mirrorYToolStripMenuItem";
            resources.ApplyResources(this.mirrorYToolStripMenuItem, "mirrorYToolStripMenuItem");
            this.mirrorYToolStripMenuItem.Click += new System.EventHandler(this.MirrorYToolStripMenuItem_Click);
            // 
            // mirrorRotaryToolStripMenuItem
            // 
            this.mirrorRotaryToolStripMenuItem.Name = "mirrorRotaryToolStripMenuItem";
            resources.ApplyResources(this.mirrorRotaryToolStripMenuItem, "mirrorRotaryToolStripMenuItem");
            this.mirrorRotaryToolStripMenuItem.Click += new System.EventHandler(this.MirrorRotaryToolStripMenuItem_Click);
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
            this.rotate90ToolStripMenuItem.Click += new System.EventHandler(this.Rotate90ToolStripMenuItem_Click);
            // 
            // rotate90ToolStripMenuItem1
            // 
            this.rotate90ToolStripMenuItem1.Name = "rotate90ToolStripMenuItem1";
            resources.ApplyResources(this.rotate90ToolStripMenuItem1, "rotate90ToolStripMenuItem1");
            this.rotate90ToolStripMenuItem1.Click += new System.EventHandler(this.Rotate90ToolStripMenuItem1_Click);
            // 
            // rotate180ToolStripMenuItem
            // 
            this.rotate180ToolStripMenuItem.Name = "rotate180ToolStripMenuItem";
            resources.ApplyResources(this.rotate180ToolStripMenuItem, "rotate180ToolStripMenuItem");
            this.rotate180ToolStripMenuItem.Click += new System.EventHandler(this.Rotate180ToolStripMenuItem_Click);
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
            resources.ApplyResources(this.toolStrip_tb_rotate, "toolStrip_tb_rotate");
            this.toolStrip_tb_rotate.Name = "toolStrip_tb_rotate";
            this.toolStrip_tb_rotate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_rotate_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_XY_scale, "toolStrip_tb_XY_scale");
            this.toolStrip_tb_XY_scale.Name = "toolStrip_tb_XY_scale";
            this.toolStrip_tb_XY_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_XY_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_XY_X_scale, "toolStrip_tb_XY_X_scale");
            this.toolStrip_tb_XY_X_scale.Name = "toolStrip_tb_XY_X_scale";
            this.toolStrip_tb_XY_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_XY_X_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_XY_Y_scale, "toolStrip_tb_XY_Y_scale");
            this.toolStrip_tb_XY_Y_scale.Name = "toolStrip_tb_XY_Y_scale";
            this.toolStrip_tb_XY_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_XY_Y_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_X_scale, "toolStrip_tb_X_scale");
            this.toolStrip_tb_X_scale.Name = "toolStrip_tb_X_scale";
            this.toolStrip_tb_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_X_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_X_X_scale, "toolStrip_tb_X_X_scale");
            this.toolStrip_tb_X_X_scale.Name = "toolStrip_tb_X_X_scale";
            this.toolStrip_tb_X_X_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_X_X_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_Y_scale, "toolStrip_tb_Y_scale");
            this.toolStrip_tb_Y_scale.Name = "toolStrip_tb_Y_scale";
            this.toolStrip_tb_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_Y_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_Y_Y_scale, "toolStrip_tb_Y_Y_scale");
            this.toolStrip_tb_Y_Y_scale.Name = "toolStrip_tb_Y_Y_scale";
            this.toolStrip_tb_Y_Y_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_Y_Y_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_rotary_diameter, "toolStrip_tb_rotary_diameter");
            this.toolStrip_tb_rotary_diameter.Name = "toolStrip_tb_rotary_diameter";
            this.toolStrip_tb_rotary_diameter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_rotary_diameter_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_X_A_scale, "toolStrip_tb_X_A_scale");
            this.toolStrip_tb_X_A_scale.Name = "toolStrip_tb_X_A_scale";
            this.toolStrip_tb_X_A_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_X_A_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tb_Y_A_scale, "toolStrip_tb_Y_A_scale");
            this.toolStrip_tb_Y_A_scale.Name = "toolStrip_tb_Y_A_scale";
            this.toolStrip_tb_Y_A_scale.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_Y_A_scale_KeyDown);
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
            resources.ApplyResources(this.toolStrip_tBRadiusCompValue, "toolStrip_tBRadiusCompValue");
            this.toolStrip_tBRadiusCompValue.Name = "toolStrip_tBRadiusCompValue";
            this.toolStrip_tBRadiusCompValue.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tBRadiusCompValue_KeyDown);
            // 
            // ersetzteG23DurchLinienToolStripMenuItem
            // 
            this.ersetzteG23DurchLinienToolStripMenuItem.Name = "ersetzteG23DurchLinienToolStripMenuItem";
            resources.ApplyResources(this.ersetzteG23DurchLinienToolStripMenuItem, "ersetzteG23DurchLinienToolStripMenuItem");
            this.ersetzteG23DurchLinienToolStripMenuItem.Click += new System.EventHandler(this.ErsetzteG23DurchLinienToolStripMenuItem_Click);
            // 
            // convertToPolarCoordinatesToolStripMenuItem
            // 
            this.convertToPolarCoordinatesToolStripMenuItem.Name = "convertToPolarCoordinatesToolStripMenuItem";
            resources.ApplyResources(this.convertToPolarCoordinatesToolStripMenuItem, "convertToPolarCoordinatesToolStripMenuItem");
            this.convertToPolarCoordinatesToolStripMenuItem.Click += new System.EventHandler(this.ConvertToPolarCoordinatesToolStripMenuItem_Click);
            // 
            // convertZToSspindleSpeedToolStripMenuItem
            // 
            this.convertZToSspindleSpeedToolStripMenuItem.Name = "convertZToSspindleSpeedToolStripMenuItem";
            resources.ApplyResources(this.convertZToSspindleSpeedToolStripMenuItem, "convertZToSspindleSpeedToolStripMenuItem");
            this.convertZToSspindleSpeedToolStripMenuItem.Click += new System.EventHandler(this.ConvertZToSspindleSpeedToolStripMenuItem_Click);
            // 
            // removeAnyZMoveToolStripMenuItem
            // 
            this.removeAnyZMoveToolStripMenuItem.Name = "removeAnyZMoveToolStripMenuItem";
            resources.ApplyResources(this.removeAnyZMoveToolStripMenuItem, "removeAnyZMoveToolStripMenuItem");
            this.removeAnyZMoveToolStripMenuItem.Click += new System.EventHandler(this.RemoveAnyZMoveToolStripMenuItem_Click);
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
            this.probingToolLengthToolStripMenuItem.Click += new System.EventHandler(this.EdgeFinderopen);
            // 
            // surfaceScanHeightMapToolStripMenuItem
            // 
            this.surfaceScanHeightMapToolStripMenuItem.Name = "surfaceScanHeightMapToolStripMenuItem";
            resources.ApplyResources(this.surfaceScanHeightMapToolStripMenuItem, "surfaceScanHeightMapToolStripMenuItem");
            this.surfaceScanHeightMapToolStripMenuItem.Click += new System.EventHandler(this.HeightMapToolStripMenuItem_Click);
            // 
            // jogPathCreatorToolStripMenuItem
            // 
            this.jogPathCreatorToolStripMenuItem.Name = "jogPathCreatorToolStripMenuItem";
            resources.ApplyResources(this.jogPathCreatorToolStripMenuItem, "jogPathCreatorToolStripMenuItem");
            this.jogPathCreatorToolStripMenuItem.Click += new System.EventHandler(this.JogCreatorToolStripMenuItem_Click);
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
            this.control3rdGRBLToolStripMenuItem,
            this.projectorToolStripMenuItem,
            this.grblSetupToolStripMenuItem,
            this.processAutomationToolStripMenuItem});
            this.machineToolStripMenuItem.Name = "machineToolStripMenuItem";
            resources.ApplyResources(this.machineToolStripMenuItem, "machineToolStripMenuItem");
            // 
            // edgeFinderToolStripMenuItem
            // 
            this.edgeFinderToolStripMenuItem.Name = "edgeFinderToolStripMenuItem";
            resources.ApplyResources(this.edgeFinderToolStripMenuItem, "edgeFinderToolStripMenuItem");
            this.edgeFinderToolStripMenuItem.Click += new System.EventHandler(this.EdgeFinderopen);
            // 
            // heightMapToolStripMenuItem
            // 
            this.heightMapToolStripMenuItem.Name = "heightMapToolStripMenuItem";
            resources.ApplyResources(this.heightMapToolStripMenuItem, "heightMapToolStripMenuItem");
            this.heightMapToolStripMenuItem.Click += new System.EventHandler(this.HeightMapToolStripMenuItem_Click);
            // 
            // laserToolsToolStripMenuItem
            // 
            this.laserToolsToolStripMenuItem.Name = "laserToolsToolStripMenuItem";
            resources.ApplyResources(this.laserToolsToolStripMenuItem, "laserToolsToolStripMenuItem");
            this.laserToolsToolStripMenuItem.Click += new System.EventHandler(this.Laseropen);
            // 
            // coordinateSystemsToolStripMenuItem
            // 
            this.coordinateSystemsToolStripMenuItem.Name = "coordinateSystemsToolStripMenuItem";
            resources.ApplyResources(this.coordinateSystemsToolStripMenuItem, "coordinateSystemsToolStripMenuItem");
            this.coordinateSystemsToolStripMenuItem.Click += new System.EventHandler(this.CoordSystemopen);
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
            this.toolStripMenuItem1.Click += new System.EventHandler(this.CameraToolStripMenuItem_Click);
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
            resources.ApplyResources(this.toolStrip_tb_StreamLine, "toolStrip_tb_StreamLine");
            this.toolStrip_tb_StreamLine.Name = "toolStrip_tb_StreamLine";
            this.toolStrip_tb_StreamLine.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ToolStrip_tb_StreamLine_KeyDown);
            // 
            // controlStreamingToolStripMenuItem
            // 
            this.controlStreamingToolStripMenuItem.Name = "controlStreamingToolStripMenuItem";
            resources.ApplyResources(this.controlStreamingToolStripMenuItem, "controlStreamingToolStripMenuItem");
            this.controlStreamingToolStripMenuItem.Click += new System.EventHandler(this.ControlStreamingToolStripMenuItem_Click);
            // 
            // control2ndGRBLToolStripMenuItem
            // 
            this.control2ndGRBLToolStripMenuItem.Name = "control2ndGRBLToolStripMenuItem";
            resources.ApplyResources(this.control2ndGRBLToolStripMenuItem, "control2ndGRBLToolStripMenuItem");
            this.control2ndGRBLToolStripMenuItem.Click += new System.EventHandler(this.Control2ndGRBLToolStripMenuItem_Click);
            // 
            // control3rdGRBLToolStripMenuItem
            // 
            this.control3rdGRBLToolStripMenuItem.Name = "control3rdGRBLToolStripMenuItem";
            resources.ApplyResources(this.control3rdGRBLToolStripMenuItem, "control3rdGRBLToolStripMenuItem");
            this.control3rdGRBLToolStripMenuItem.Click += new System.EventHandler(this.Control3rdSerialCOMToolStripMenuItem_Click);
            // 
            // projectorToolStripMenuItem
            // 
            this.projectorToolStripMenuItem.Name = "projectorToolStripMenuItem";
            resources.ApplyResources(this.projectorToolStripMenuItem, "projectorToolStripMenuItem");
            this.projectorToolStripMenuItem.Click += new System.EventHandler(this.ProjectorToolStripMenuItem_Click);
            // 
            // grblSetupToolStripMenuItem
            // 
            this.grblSetupToolStripMenuItem.Name = "grblSetupToolStripMenuItem";
            resources.ApplyResources(this.grblSetupToolStripMenuItem, "grblSetupToolStripMenuItem");
            this.grblSetupToolStripMenuItem.Click += new System.EventHandler(this.GrblSetupToolStripMenuItem_Click);
            // 
            // processAutomationToolStripMenuItem
            // 
            this.processAutomationToolStripMenuItem.Name = "processAutomationToolStripMenuItem";
            resources.ApplyResources(this.processAutomationToolStripMenuItem, "processAutomationToolStripMenuItem");
            this.processAutomationToolStripMenuItem.Click += new System.EventHandler(this.ProcessAutomationFormOpen);
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
            this.toolStripViewRuler.Checked = true;
            this.toolStripViewRuler.CheckOnClick = true;
            this.toolStripViewRuler.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewRuler.Name = "toolStripViewRuler";
            resources.ApplyResources(this.toolStripViewRuler, "toolStripViewRuler");
            this.toolStripViewRuler.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewInfo
            // 
            this.toolStripViewInfo.Checked = true;
            this.toolStripViewInfo.CheckOnClick = true;
            this.toolStripViewInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewInfo.Name = "toolStripViewInfo";
            resources.ApplyResources(this.toolStripViewInfo, "toolStripViewInfo");
            this.toolStripViewInfo.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewPenUp
            // 
            this.toolStripViewPenUp.Checked = true;
            this.toolStripViewPenUp.CheckOnClick = true;
            this.toolStripViewPenUp.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewPenUp.Name = "toolStripViewPenUp";
            resources.ApplyResources(this.toolStripViewPenUp, "toolStripViewPenUp");
            this.toolStripViewPenUp.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewMachineFix
            // 
            this.toolStripViewMachineFix.CheckOnClick = true;
            this.toolStripViewMachineFix.Name = "toolStripViewMachineFix";
            resources.ApplyResources(this.toolStripViewMachineFix, "toolStripViewMachineFix");
            this.toolStripViewMachineFix.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewMachine
            // 
            this.toolStripViewMachine.Checked = true;
            this.toolStripViewMachine.CheckOnClick = true;
            this.toolStripViewMachine.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewMachine.Name = "toolStripViewMachine";
            resources.ApplyResources(this.toolStripViewMachine, "toolStripViewMachine");
            this.toolStripViewMachine.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewDimension
            // 
            this.toolStripViewDimension.Checked = true;
            this.toolStripViewDimension.CheckOnClick = true;
            this.toolStripViewDimension.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewDimension.Name = "toolStripViewDimension";
            resources.ApplyResources(this.toolStripViewDimension, "toolStripViewDimension");
            this.toolStripViewDimension.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewTool
            // 
            this.toolStripViewTool.CheckOnClick = true;
            this.toolStripViewTool.Name = "toolStripViewTool";
            resources.ApplyResources(this.toolStripViewTool, "toolStripViewTool");
            this.toolStripViewTool.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // toolStripViewBackground
            // 
            this.toolStripViewBackground.Checked = true;
            this.toolStripViewBackground.CheckOnClick = true;
            this.toolStripViewBackground.CheckState = System.Windows.Forms.CheckState.Checked;
            this.toolStripViewBackground.Name = "toolStripViewBackground";
            resources.ApplyResources(this.toolStripViewBackground, "toolStripViewBackground");
            this.toolStripViewBackground.Click += new System.EventHandler(this.UpdatePathDisplay);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            resources.ApplyResources(this.logToolStripMenuItem, "logToolStripMenuItem");
            // 
            // showFormsToolStripMenuItem
            // 
            this.showFormsToolStripMenuItem.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.showFormsToolStripMenuItem.Name = "showFormsToolStripMenuItem";
            resources.ApplyResources(this.showFormsToolStripMenuItem, "showFormsToolStripMenuItem");
            this.showFormsToolStripMenuItem.Click += new System.EventHandler(this.ShowFormsToolStripMenuItem_Click);
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
            this.gamePadTimer.Tick += new System.EventHandler(this.GamePadTimer_Tick);
            // 
            // simulationTimer
            // 
            this.simulationTimer.Interval = 50;
            this.simulationTimer.Tick += new System.EventHandler(this.SimulationTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
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
            this.loadTimer.Interval = 200;
            this.loadTimer.Tick += new System.EventHandler(this.LoadTimer_Tick);
            // 
            // cmsPictureBox2
            // 
            this.cmsPictureBox2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsPictureBox2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cms2ToolStripMenuItem,
            this.offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem,
            this.toolStripSeparator18,
            this.moveBetweenLastPositionsToolStripMenuItem});
            this.cmsPictureBox2.Name = "cmsPictureBox2";
            resources.ApplyResources(this.cmsPictureBox2, "cmsPictureBox2");
            // 
            // cms2ToolStripMenuItem
            // 
            this.cms2ToolStripMenuItem.Name = "cms2ToolStripMenuItem";
            resources.ApplyResources(this.cms2ToolStripMenuItem, "cms2ToolStripMenuItem");
            this.cms2ToolStripMenuItem.Click += new System.EventHandler(this.CmsPicBoxMoveGraphicsOrigin_Click);
            // 
            // offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem
            // 
            this.offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem.Name = "offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem";
            resources.ApplyResources(this.offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem, "offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem");
            this.offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem.Click += new System.EventHandler(this.CmsPicBoxMoveGraphicsOrigin2_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // moveBetweenLastPositionsToolStripMenuItem
            // 
            this.moveBetweenLastPositionsToolStripMenuItem.CheckOnClick = true;
            this.moveBetweenLastPositionsToolStripMenuItem.Name = "moveBetweenLastPositionsToolStripMenuItem";
            resources.ApplyResources(this.moveBetweenLastPositionsToolStripMenuItem, "moveBetweenLastPositionsToolStripMenuItem");
            this.moveBetweenLastPositionsToolStripMenuItem.Click += new System.EventHandler(this.MoveBetweenLastPositionsToolStripMenuItem_Click);
            // 
            // moveTimer
            // 
            this.moveTimer.Interval = 200;
            this.moveTimer.Tick += new System.EventHandler(this.MoveTimer_Tick);
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
            // 
            // tLPLinks
            // 
            resources.ApplyResources(this.tLPLinks, "tLPLinks");
            this.tLPLinks.Controls.Add(this.ucStreaming, 0, 0);
            this.tLPLinks.Controls.Add(this.ucOverrides, 0, 1);
            this.tLPLinks.Controls.Add(this.fCTBCode, 0, 4);
            this.tLPLinks.Controls.Add(this.ucFlowControl, 0, 2);
            this.tLPLinks.Controls.Add(this.ucSetOffset, 0, 3);
            this.tLPLinks.Name = "tLPLinks";
            // 
            // ucStreaming
            // 
            this.ucStreaming.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.ucStreaming, "ucStreaming");
            this.ucStreaming.Name = "ucStreaming";
            // 
            // ucOverrides
            // 
            resources.ApplyResources(this.ucOverrides, "ucOverrides");
            this.ucOverrides.Name = "ucOverrides";
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
            this.fCTBCode.ToolTip = this.toolTip1;
            this.fCTBCode.Zoom = 100;
            this.fCTBCode.ToolTipNeeded += new System.EventHandler<FastColoredTextBoxNS.ToolTipNeededEventArgs>(this.FctbCode_ToolTipNeeded);
            this.fCTBCode.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.FctbCode_TextChanged);
            this.fCTBCode.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.FctbCode_TextChangedDelayed);
            this.fCTBCode.Click += new System.EventHandler(this.FctbCode_Click);
            this.fCTBCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FctbCode_KeyDown);
            this.fCTBCode.MouseHover += new System.EventHandler(this.FCTBCode_MouseHover);
            // 
            // tBURL
            // 
            resources.ApplyResources(this.tBURL, "tBURL");
            this.tBURL.Name = "tBURL";
            this.toolTip1.SetToolTip(this.tBURL, resources.GetString("tBURL.ToolTip"));
            this.tBURL.TextChanged += new System.EventHandler(this.TbURL_TextChanged);
            // 
            // Rb2DViewMode1
            // 
            resources.ApplyResources(this.Rb2DViewMode1, "Rb2DViewMode1");
            this.Rb2DViewMode1.BackColor = System.Drawing.Color.Yellow;
            this.Rb2DViewMode1.Checked = true;
            this.Rb2DViewMode1.Name = "Rb2DViewMode1";
            this.Rb2DViewMode1.TabStop = true;
            this.toolTip1.SetToolTip(this.Rb2DViewMode1, resources.GetString("Rb2DViewMode1.ToolTip"));
            this.Rb2DViewMode1.UseVisualStyleBackColor = false;
            this.Rb2DViewMode1.CheckedChanged += new System.EventHandler(this.Rb2DViewMode1_CheckedChanged);
            // 
            // Rb2DViewMode2
            // 
            resources.ApplyResources(this.Rb2DViewMode2, "Rb2DViewMode2");
            this.Rb2DViewMode2.Name = "Rb2DViewMode2";
            this.toolTip1.SetToolTip(this.Rb2DViewMode2, resources.GetString("Rb2DViewMode2.ToolTip"));
            this.Rb2DViewMode2.UseVisualStyleBackColor = true;
            this.Rb2DViewMode2.CheckedChanged += new System.EventHandler(this.Rb2DViewMode2_CheckedChanged);
            // 
            // Rb2DViewMode3
            // 
            resources.ApplyResources(this.Rb2DViewMode3, "Rb2DViewMode3");
            this.Rb2DViewMode3.Name = "Rb2DViewMode3";
            this.toolTip1.SetToolTip(this.Rb2DViewMode3, resources.GetString("Rb2DViewMode3.ToolTip"));
            this.Rb2DViewMode3.UseVisualStyleBackColor = true;
            this.Rb2DViewMode3.CheckedChanged += new System.EventHandler(this.Rb2DViewMode2_CheckedChanged);
            // 
            // CbAddGraphic
            // 
            resources.ApplyResources(this.CbAddGraphic, "CbAddGraphic");
            settings1.cameraColorCross = System.Drawing.Color.Yellow;
            settings1.cameraColorText = System.Drawing.Color.DimGray;
            settings1.cameraDistortionEnable = true;
            settings1.cameraDistortionFixP0 = new System.Drawing.Point(64, 53);
            settings1.cameraDistortionFixP1 = new System.Drawing.Point(579, 43);
            settings1.cameraDistortionFixP2 = new System.Drawing.Point(602, 436);
            settings1.cameraDistortionFixP3 = new System.Drawing.Point(33, 433);
            settings1.cameraDistortionXyP0 = new System.Drawing.Point(0, 0);
            settings1.cameraDistortionXyP1 = new System.Drawing.Point(640, 0);
            settings1.cameraDistortionXyP2 = new System.Drawing.Point(640, 480);
            settings1.cameraDistortionXyP3 = new System.Drawing.Point(0, 480);
            settings1.cameraFilterIndex = "0";
            settings1.cameraFilterIndexFix = ((byte)(0));
            settings1.cameraFilterIndexXy = ((byte)(0));
            settings1.cameraIndexFix = ((byte)(0));
            settings1.cameraIndexNameFix = "";
            settings1.cameraIndexNameXy = "";
            settings1.cameraIndexXy = ((byte)(0));
            settings1.cameraMount = 0;
            settings1.cameraPosBot = -50D;
            settings1.cameraPosTop = 0D;
            settings1.cameraRotateArround0 = true;
            settings1.cameraRotationFix = 180D;
            settings1.cameraRotationXy = 0D;
            settings1.cameraScaleOnRotate = false;
            settings1.cameraScalingFix = 2.274D;
            settings1.cameraScalingXy = 20D;
            settings1.cameraScalingXyzBot = 15D;
            settings1.cameraScalingXyzTop = 20D;
            settings1.cameraShowPathDimension = false;
            settings1.cameraShowPathLimits = false;
            settings1.cameraShowPathPenUp = false;
            settings1.cameraSimulateImage = false;
            settings1.cameraTeachRadiusFix = 200D;
            settings1.cameraTeachRadiusXy = 20D;
            settings1.cameraTeachRadiusXyzBot = 20D;
            settings1.cameraTeachRadiusXyzTop = 30D;
            settings1.cameraToolOffsetX = 0D;
            settings1.cameraToolOffsetY = 0D;
            settings1.cameraZeroFixMachineX = 0D;
            settings1.cameraZeroFixMachineY = 0D;
            settings1.cameraZeroFixX = 96D;
            settings1.cameraZeroFixY = 420D;
            settings1.camFilterBlue1 = 0;
            settings1.camFilterBlue2 = 64;
            settings1.camFilterGreen1 = 0;
            settings1.camFilterGreen2 = 64;
            settings1.camFilterOutside = false;
            settings1.camFilterRed1 = 0;
            settings1.camFilterRed2 = 64;
            settings1.camShapeAutoTimeout = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.camShapeCircle = true;
            settings1.camShapeDist = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.camShapeDistMax = new decimal(new int[] {
            3,
            0,
            0,
            131072});
            settings1.camShapeRect = true;
            settings1.camShapeSet1 = "Fiducials|0|90|0|90|0|120|False|True|False|8|14|2|2|";
            settings1.camShapeSet2 = "Paper|0|100|0|100|0|100|True|True|True|5|50|0.5|1|";
            settings1.camShapeSet3 = "PCB|0|150|0|150|0|150|False|False|True|1|6|0.5|1|";
            settings1.camShapeSet4 = "Wood|0|106|0|237|123|246|True|True|False|5|20|0.5|2.0|";
            settings1.camShapeSize = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.camShapeSizeMax = new decimal(new int[] {
            15,
            0,
            0,
            0});
            settings1.camShapeSizeMin = new decimal(new int[] {
            8,
            0,
            0,
            0});
            settings1.colorPaletteLastLoaded = "GRBL-Plotter_Watercolor.txt";
            settings1.convertZtoSMax = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.convertZtoSMin = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.convertZtoSOff = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.counterImportBarcode = ((uint)(0u));
            settings1.counterImportCSV = ((uint)(0u));
            settings1.counterImportDrill = ((uint)(0u));
            settings1.counterImportDXF = ((uint)(0u));
            settings1.counterImportExtension = ((uint)(0u));
            settings1.counterImportGCode = ((uint)(0u));
            settings1.counterImportGerber = ((uint)(0u));
            settings1.counterImportHPGL = ((uint)(0u));
            settings1.counterImportImage = ((uint)(0u));
            settings1.counterImportPDNJson = ((uint)(0u));
            settings1.counterImportShape = ((uint)(0u));
            settings1.counterImportSVG = ((uint)(0u));
            settings1.counterImportText = ((uint)(0u));
            settings1.counterUseHeightMap = ((uint)(0u));
            settings1.counterUseLaserSetup = ((uint)(0u));
            settings1.counterUseProbing = ((uint)(0u));
            settings1.crcValue = 2.5D;
            settings1.createJogPathCodeEnd = "STOP";
            settings1.createJogPathCodeStart = "M3 S1000;G4 P1;";
            settings1.createJogPathFeedrate = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.createJogPathRaster = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.createShapeMovo00 = false;
            settings1.createShapeNoZUp = false;
            settings1.createShapeOrigin = 5;
            settings1.createShapeR = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.createShapeRZRadius = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.createShapeRZStep = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.createShapeRZWidth = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.createShapeToolDiameter = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.createShapeToolFeedXY = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.createShapeToolFeedZ = new decimal(new int[] {
            400,
            0,
            0,
            0});
            settings1.createShapeToolOverlap = new decimal(new int[] {
            90,
            0,
            0,
            0});
            settings1.createShapeToolPath = 1;
            settings1.createShapeToolSpindleSpeed = new decimal(new int[] {
            20000,
            0,
            0,
            0});
            settings1.createShapeToolZStep = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.createShapeType = 1;
            settings1.createShapeX = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.createShapeY = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.createTextAlignment = 1;
            settings1.createTextFontColor = "WindowText";
            settings1.createTextFontIndex = 0;
            settings1.createTextFontText = "Test";
            settings1.createTextHersheyFontName = "cBFont";
            settings1.createTextHersheyFontSize = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.createTextHersheyLetterDistance = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.createTextHersheyLineBreak = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.createTextHersheyLineBreakEnable = false;
            settings1.createTextHersheyLineDistance = new decimal(new int[] {
            15,
            0,
            0,
            0});
            settings1.createTextHersheySelect = true;
            settings1.createTextSystemFont = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            settings1.createTextSystemFontSize = 16F;
            settings1.createTextSystemSizeX = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.createTextSystemSizeY = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.ctrl4thInvert = false;
            settings1.ctrl4thName = "A";
            settings1.ctrl4thOverX = true;
            settings1.ctrl4thUse = false;
            settings1.ctrlColorizeGCode = true;
            settings1.ctrlCommentOut = true;
            settings1.ctrlConnectMarlin = false;
            settings1.ctrlDIYShowFeedback = true;
            settings1.ctrlImportSkip = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.ctrlLaserMode = false;
            settings1.ctrlLineEndEnable = false;
            settings1.ctrlLineEndText = ";";
            settings1.ctrlLineNumbers = false;
            settings1.ctrlReplaceEnable = false;
            settings1.ctrlReplaceM3 = true;
            settings1.ctrlSendStopJog = true;
            settings1.ctrlToolChange = false;
            settings1.ctrlToolChangeEmpty = false;
            settings1.ctrlToolChangeEmptyNr = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.ctrlToolChangeM6PassThrough = false;
            settings1.ctrlToolScriptDelay = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.ctrlToolScriptGet = "data\\scripts\\script_v1583_brush_select.nc";
            settings1.ctrlToolScriptProbe = "data\\scripts\\";
            settings1.ctrlToolScriptPut = "data\\scripts\\";
            settings1.ctrlToolScriptSelect = "data\\scripts\\script_v1583_brush_clean.nc";
            settings1.ctrlUpgradeRequired = true;
            settings1.ctrlUseSerial2 = false;
            settings1.ctrlUseSerial3 = false;
            settings1.ctrlUseSerialDIY = false;
            settings1.ctrlUseSerialPortFixer = true;
            settings1.DeviceLaserAir = false;
            settings1.DeviceLaserCmndAirOff = "M9";
            settings1.DeviceLaserCmndAirOn = "M8";
            settings1.DeviceLaserCmndPilotOff = "M9";
            settings1.DeviceLaserCmndPilotOn = "M7";
            settings1.DeviceLaserHatchFillAngle = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DeviceLaserHatchFillAngleIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DeviceLaserHatchFillAngleIncrementEnable = false;
            settings1.DeviceLaserHatchFillDeletePath = false;
            settings1.DeviceLaserHatchFillDistance = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.DeviceLaserHatchFillEnable = false;
            settings1.DeviceLaserHatchFillInsetDistance = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.DeviceLaserM3 = true;
            settings1.DeviceLaserPasses = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.DeviceLaserPower = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.DeviceLaserPowerLow = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.DeviceLaserPowerMin = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DeviceLaserSpeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.DeviceLaserSplitterDistance = 285;
            settings1.DeviceLaserToolDiameter = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.DeviceLaserZDown = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DeviceLaserZEnable = false;
            settings1.DeviceLaserZFeed = new decimal(new int[] {
            200,
            0,
            0,
            0});
            settings1.DeviceLaserZSave = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.DevicePlotterControlIndex = 0;
            settings1.DevicePlotterDepthControl = false;
            settings1.DevicePlotterHatchFillAngle = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.DevicePlotterHatchFillAngleIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DevicePlotterHatchFillAngleIncrementEnable = false;
            settings1.DevicePlotterHatchFillDeletePath = false;
            settings1.DevicePlotterHatchFillDistance = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.DevicePlotterHatchFillEnable = false;
            settings1.DevicePlotterHatchFillInsetDistance = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.DevicePlotterPenChangeRBAutomatic = false;
            settings1.DevicePlotterPenChangeRBManual = false;
            settings1.DevicePlotterPenChangeRBNo = true;
            settings1.DevicePlotterPenInHolder = false;
            settings1.DevicePlotterSpeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.DevicePlotterSpeedZ = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.DevicePlotterToolDiameter = new decimal(new int[] {
            8,
            0,
            0,
            65536});
            settings1.DevicePlotterZDown = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            settings1.DevicePlotterZP93 = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DevicePlotterZP94 = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.DevicePlotterZUp = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.DeviceRouterSpeedXY = new decimal(new int[] {
            200,
            0,
            0,
            0});
            settings1.DeviceRouterSpeedZ = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.DeviceRouterSpindle = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.DeviceRouterToolDiameter = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.DeviceRouterZDown = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            settings1.DeviceRouterZUp = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.DeviceSplitterDistance = 300;
            settings1.FCTBBlockExpandKeepLastOpen = false;
            settings1.FCTBBlockExpandOnSelect = true;
            settings1.FCTBLineInterval = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.FCTBSaveEncodingIndex = 0;
            settings1.FCTBSaveWithoutComments = false;
            settings1.flowCheckRegistryChange = true;
            settings1.flowControlEnable = true;
            settings1.flowControlText = "M5; M9";
            settings1.fromFormInsertEnable = false;
            settings1.gamePadAnalogDead = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.gamePadAnalogMinFeed = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.gamePadAnalogMinimum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            settings1.gamePadAnalogMinStep = new decimal(new int[] {
            10,
            0,
            0,
            196608});
            settings1.gamePadAnalogOffset = new decimal(new int[] {
            32757,
            0,
            0,
            0});
            settings1.gamePadButtons0 = "#Start (Start streaming)";
            settings1.gamePadButtons1 = "#F+10 (Feedrate +10%)";
            settings1.gamePadButtons10 = "G92 X0 Y0 (Zero X and Y)";
            settings1.gamePadButtons11 = "";
            settings1.gamePadButtons12 = "";
            settings1.gamePadButtons13 = "";
            settings1.gamePadButtons14 = "";
            settings1.gamePadButtons15 = "";
            settings1.gamePadButtons2 = "#F100 (Feedrate =100%)";
            settings1.gamePadButtons3 = "#F-10 (Feedrate -10%)";
            settings1.gamePadButtons4 = "G92 X0 (Zero X)";
            settings1.gamePadButtons5 = "G92 Z0 (Zero Z)";
            settings1.gamePadButtons6 = "G92 Y0 (Zero Y)";
            settings1.gamePadButtons7 = "G92 X0 Y0 Z0 (Zero all)";
            settings1.gamePadButtons8 = "$H (Homing)";
            settings1.gamePadButtons9 = "#Start (Start streaming)";
            settings1.gamePadEnable = false;
            settings1.gamePadPOVC00 = "$J=G91 Y1.000 F100";
            settings1.gamePadPOVC01 = "$J=G91 X1.000 Y1.000 F100";
            settings1.gamePadPOVC02 = "$J=G91 X1.000 F100";
            settings1.gamePadPOVC03 = "$J=G91 X1.000 Y-1.000 F100";
            settings1.gamePadPOVC04 = "$J=G91 Y-1.000 F100";
            settings1.gamePadPOVC05 = "$J=G91 X-1.000 Y-1.000 F100";
            settings1.gamePadPOVC06 = "$J=G91 X-1.000 F100";
            settings1.gamePadPOVC07 = "$J=G91 X-1.000 Y1.000 F100";
            settings1.gamePadRAxis = "A";
            settings1.gamePadRInvert = false;
            settings1.gamePadXAxis = "X";
            settings1.gamePadXInvert = false;
            settings1.gamePadYAxis = "Y";
            settings1.gamePadYInvert = false;
            settings1.gamePadZAxis = "Z";
            settings1.gamePadZInvert = false;
            settings1.grblBufferAutomatic = true;
            settings1.grblBufferSize = new decimal(new int[] {
            127,
            0,
            0,
            0});
            settings1.grblDescriptionD0 = "D0";
            settings1.grblDescriptionD1 = "D1";
            settings1.grblDescriptionD2 = "D2";
            settings1.grblDescriptionD3 = "D3";
            settings1.grblDescriptionDxEnable = false;
            settings1.grblLastOffsetA = 0D;
            settings1.grblLastOffsetB = 0D;
            settings1.grblLastOffsetC = 0D;
            settings1.grblLastOffsetCoord = 54;
            settings1.grblLastOffsetX = 0D;
            settings1.grblLastOffsetY = 0D;
            settings1.grblLastOffsetZ = 0D;
            settings1.grblPollIntervalIndex = 2;
            settings1.grblPollIntervalReduce = false;
            settings1.grblRunTimeFlood = ((long)(0));
            settings1.grblRunTimeMist = ((long)(0));
            settings1.grblRunTimeSpindle = ((long)(0));
            settings1.grblStreamingProtocol1 = true;
            settings1.grblTranslateMessage = false;
            settings1.gui2DColorBackground = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            settings1.gui2DColorBackgroundPath = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            settings1.gui2DColorDimension = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            settings1.gui2DColorHeightMap = System.Drawing.Color.Yellow;
            settings1.gui2DColorMachineLimit = System.Drawing.Color.Fuchsia;
            settings1.gui2DColorMarker = System.Drawing.Color.DeepPink;
            settings1.gui2DColorPenDown = System.Drawing.Color.Red;
            settings1.gui2DColorPenDownModeEnable = true;
            settings1.gui2DColorPenDownModeWidth = true;
            settings1.gui2DColorPenUp = System.Drawing.Color.Green;
            settings1.gui2DColorRotaryInfo = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            settings1.gui2DColorRuler = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            settings1.gui2DColorSimulation = System.Drawing.Color.Lime;
            settings1.gui2DColorTool = System.Drawing.Color.Black;
            settings1.gui2DDuplicateAddDimensionX = true;
            settings1.gui2DDuplicateAddDimensionY = false;
            settings1.gui2DDuplicateOffsetX = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.gui2DDuplicateOffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.gui2DInfoShow = true;
            settings1.gui2DInfoSize1 = new decimal(new int[] {
            7,
            0,
            0,
            0});
            settings1.gui2DInfoSize2 = new decimal(new int[] {
            8,
            0,
            0,
            0});
            settings1.gui2DInfoTranparency = 128;
            settings1.gui2DKeepPenWidth = false;
            settings1.gui2DPenUpArrow = true;
            settings1.gui2DPenUpId = true;
            settings1.gui2DPenUpShow = false;
            settings1.gui2DRulerShow = true;
            settings1.gui2DShowVertexEnable = false;
            settings1.gui2DShowVertexSize = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.gui2DShowVertexType = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.gui2DSizeTool = new decimal(new int[] {
            40,
            0,
            0,
            0});
            settings1.gui2DToolTableShow = false;
            settings1.gui2DWidthHeightMap = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.gui2DWidthMarker = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.gui2DWidthPenDown = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.gui2DWidthPenUp = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.gui2DWidthRotaryInfo = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.gui2DWidthRuler = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.gui2DWidthSimulation = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.gui2DWidthTool = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.guiBackgroundImageEnable = false;
            settings1.guiBackgroundShow = true;
            settings1.guiCheckUpdate = true;
            settings1.guiCheckUpdateURL = "https://GRBL-Plotter.de";
            settings1.guiColorSchemeEnable = false;
            settings1.guiColorThemeButton = System.Drawing.Color.Empty;
            settings1.guiColorThemeButtonH = 0;
            settings1.guiColorThemeButtonS = 100;
            settings1.guiColorThemeButtonV = 100;
            settings1.guiColorThemeEnable = false;
            settings1.guiColorThemePanel = System.Drawing.Color.Empty;
            settings1.guiColorThemePanelH = 0;
            settings1.guiColorThemePanelS = 100;
            settings1.guiColorThemePanelV = 100;
            settings1.guiCustomBtn1 = "🡴 Graphic Top-Left|(Move to upper left corner - jog fast);$J=G90 F5000 X#GMIX Y#" +
    "GMAY|#80FFFF";
            settings1.guiCustomBtn10 = "▼  Pen down Zmin|(Move Pen to lower position);G90 G0 Z#GMIZ|#FF8080";
            settings1.guiCustomBtn11 = "🡶 Graphic Bottom-Right|(Move to lower right corner - jog fast);$J=G90 F5000 X#GM" +
    "AX Y#GMIY|#80FFFF";
            settings1.guiCustomBtn12 = "▼ Pen down Servo|(Move Pen to lower position);M3 S#GMIS|#FF8080";
            settings1.guiCustomBtn13 = " |";
            settings1.guiCustomBtn14 = " | ";
            settings1.guiCustomBtn15 = " | ";
            settings1.guiCustomBtn16 = " | ";
            settings1.guiCustomBtn17 = " |";
            settings1.guiCustomBtn18 = " | ";
            settings1.guiCustomBtn19 = " | ";
            settings1.guiCustomBtn2 = "▲ Pen up Zmax|(Move Pen to upper position);$J=G90 F5000 Z#GMAZ|#80FF80";
            settings1.guiCustomBtn20 = " | ";
            settings1.guiCustomBtn21 = " | ";
            settings1.guiCustomBtn22 = " | ";
            settings1.guiCustomBtn23 = " | ";
            settings1.guiCustomBtn24 = " | ";
            settings1.guiCustomBtn25 = " | ";
            settings1.guiCustomBtn26 = " | ";
            settings1.guiCustomBtn27 = " | ";
            settings1.guiCustomBtn28 = " | ";
            settings1.guiCustomBtn29 = " | ";
            settings1.guiCustomBtn3 = "🡷 Graphic Bottom-Left|(Move to lower left corner - jog fast);$J=G90 F5000 X#GMIX" +
    " Y#GMIY|#80FFFF";
            settings1.guiCustomBtn30 = " | ";
            settings1.guiCustomBtn31 = " | ";
            settings1.guiCustomBtn32 = " | ";
            settings1.guiCustomBtn4 = "▲ Pen up Servo|(Move Pen to upper position);M3 S#GMAS|#80FF80";
            settings1.guiCustomBtn5 = "↻ Move around graphic|(Move around graphic dimension - jog fast);G90G1F5000X#GMIX" +
    "Y#GMIY;Y#GMAY;X#GMAX;Y#GMIY;X#GMIX|#FFFF80";
            settings1.guiCustomBtn6 = "◎ Graphic Center|(Move to graphic center - fast);$J=G90 F5000 X#GCTX Y#GCTY|#00FF" +
    "FF";
            settings1.guiCustomBtn7 = "Add Logo|data\\scripts\\script_my_logo.nc|#FFFFC0";
            settings1.guiCustomBtn8 = "► Pen zero Servo|(Move Pen to zero position);M3 S#GZES|#e6f7ff";
            settings1.guiCustomBtn9 = "🡵 Graphic Top-Right|(Move to upper right corner - jog fast);$J=G90 F5000 X#GMAX " +
    "Y#GMAY|#80FFFF";
            settings1.guiDimensionShow = true;
            settings1.guiDisableProgramPause = false;
            settings1.guiExtendedLoggingCOMEnabled = false;
            settings1.guiExtendedLoggingEnabled = false;
            settings1.guiJoystickASpeed1 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.guiJoystickASpeed2 = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.guiJoystickASpeed3 = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.guiJoystickASpeed4 = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.guiJoystickASpeed5 = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.guiJoystickAStep1 = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.guiJoystickAStep2 = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.guiJoystickAStep3 = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.guiJoystickAStep4 = new decimal(new int[] {
            45,
            0,
            0,
            0});
            settings1.guiJoystickAStep5 = new decimal(new int[] {
            180,
            0,
            0,
            0});
            settings1.guiJoystickRaster = 5;
            settings1.guiJoystickXYSpeed1 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.guiJoystickXYSpeed2 = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.guiJoystickXYSpeed3 = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.guiJoystickXYSpeed4 = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.guiJoystickXYSpeed5 = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.guiJoystickXYStep1 = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.guiJoystickXYStep2 = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.guiJoystickXYStep3 = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.guiJoystickXYStep4 = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.guiJoystickXYStep5 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.guiJoystickZSpeed1 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.guiJoystickZSpeed2 = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.guiJoystickZSpeed3 = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.guiJoystickZSpeed4 = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.guiJoystickZSpeed5 = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.guiJoystickZStep1 = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.guiJoystickZStep2 = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.guiJoystickZStep3 = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.guiJoystickZStep4 = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.guiJoystickZStep5 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.guiLanguage = "en";
            settings1.guiLastEnd = ((long)(0));
            settings1.guiLastEndReason = "-";
            settings1.guiLastFileLoaded = "D:\\";
            settings1.guiLastStart = ((long)(0));
            settings1.guiPathSaveCode = "";
            settings1.guiPenUpDownButtonsShow = false;
            settings1.guiProgressShow = true;
            settings1.guiShowFormInFront = false;
            settings1.guiSpindleSpeed = "1000";
            settings1.guiToolSelection = 0;
            settings1.heightMapDeltaX = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.heightMapDeltaY = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.heightMapExtrudeEnable = false;
            settings1.heightMapExtrudeScanX = true;
            settings1.heightMapGridX = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.heightMapGridY = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.heightMapProbeDepth = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            settings1.heightMapProbeG1 = false;
            settings1.heightMapProbeHeight = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.heightMapProbeSpeed = new decimal(new int[] {
            400,
            0,
            0,
            0});
            settings1.heightMapProbeSpeedXY = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            settings1.heightMapProbeUseZ = true;
            settings1.heightMapX1 = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.heightMapX2 = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.heightMapY1 = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.heightMapY2 = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.importAssumeAsEqualDistance = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            settings1.importBarcode1DHeight = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.importBarcode1DLines = new decimal(new int[] {
            4,
            0,
            0,
            0});
            settings1.importBarcode1DName = "";
            settings1.importBarcode1DScanGap = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.importBarcode1DSelect = true;
            settings1.importBarcode1DText = "123";
            settings1.importBarcode2DBorder = true;
            settings1.importBarcode2DFacebook = "";
            settings1.importBarcode2DHeight = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.importBarcode2DLines = new decimal(new int[] {
            4,
            0,
            0,
            0});
            settings1.importBarcode2DScanGap = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.importBarcode2DText = "123";
            settings1.importBarcode2DTwitter = "#plottertwitter";
            settings1.importBarcode2DURL = "https://grbl-plotter.de/";
            settings1.importBarcode2DWlanPass = "unknown";
            settings1.importBarcode2DWlanSSID = "my_wlan";
            settings1.importBarcode2DYoutube = "UCxiLZDTcWmNzBk5ksgC7_Rg";
            settings1.importBezierLineSegmentsCnt = new decimal(new int[] {
            12,
            0,
            0,
            0});
            settings1.importCircleToDotScript = "data\\scripts\\script_stamp_refresh.nc";
            settings1.importCircleToDotScriptCount = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importCodeFold = true;
            settings1.importCSVAutomatic = true;
            settings1.importCSVColumnX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importCSVColumnY = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importCSVColumnZ = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importCSVDelimeter = ";";
            settings1.importCSVProzessAsLine = true;
            settings1.importCSVProzessZ = false;
            settings1.importCSVScaleX = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importCSVScaleY = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importCSVScaleZ = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importCSVStartLine = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importDepthFromWidth = false;
            settings1.importDepthFromWidthMax = new decimal(new int[] {
            5,
            0,
            0,
            -2147483648});
            settings1.importDepthFromWidthMin = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            settings1.importDepthFromWidthRamp = false;
            settings1.importDXFDontPlot = false;
            settings1.ImportDXFSize = true;
            settings1.importDXFSwitchWhite = true;
            settings1.importDXFToolIndex = false;
            settings1.importDXFUseZ = false;
            settings1.importFiducialLabel = "fiducial";
            settings1.importFiducialSkipCode = false;
            settings1.importFigureMaxAmount = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.importGCAddComments = false;
            settings1.importGCAux1Axis = "A";
            settings1.importGCAux1Enable = false;
            settings1.importGCAux1Factor = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCAux1Source = 1;
            settings1.importGCAux1SumUp = true;
            settings1.importGCAux1ZFactor = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCAux1ZMode = 0;
            settings1.importGCAux1ZUse = false;
            settings1.importGCAux2Axis = "B";
            settings1.importGCAux2Enable = false;
            settings1.importGCAux2Factor = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCAux2Source = 1;
            settings1.importGCAux2SumUp = true;
            settings1.importGCAux2ZFactor = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCAux2ZMode = 0;
            settings1.importGCAux2ZUse = false;
            settings1.importGCCompress = false;
            settings1.importGCConvertToPolar = false;
            settings1.importGCConvertToPolarAccuracy = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.importGCDecPlaces = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.importGCDragKnifeAngle = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.importGCDragKnifeEnable = false;
            settings1.importGCDragKnifeLength = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGCDragKnifePercent = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.importGCDragKnifePercentEnable = false;
            settings1.importGCDragKnifeUse = false;
            settings1.importGCFooter = "";
            settings1.importGCHeader = "G54; ";
            settings1.importGCIndEnable = false;
            settings1.importGCIndPenDown = "(Pen Down command);(Cmd2)";
            settings1.importGCIndPenUp = "(Pen Up command);(Cmd2)";
            settings1.importGCLineSegmentation = false;
            settings1.importGCLineSegmentEquidistant = false;
            settings1.importGCLineSegmentLength = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.importGCNoArcs = false;
            settings1.importGCPWMDlyDown = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGCPWMDlyP93 = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGCPWMDlyP94 = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGCPWMDlyUp = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGCPWMDown = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.importGCPWMEnable = false;
            settings1.importGCPWMP93 = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCPWMP94 = new decimal(new int[] {
            15,
            0,
            0,
            0});
            settings1.importGCPWMSkipM30 = false;
            settings1.importGCPWMTextP93 = "Far up";
            settings1.importGCPWMTextP94 = "Down stir";
            settings1.importGCPWMUp = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.importGCPWMZero = new decimal(new int[] {
            25,
            0,
            0,
            0});
            settings1.importGCRelative = false;
            settings1.importGCSDirM3 = true;
            settings1.importGCSegment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGCSpindleCmd = true;
            settings1.importGCSpindleDelay = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGCSpindleToggle = false;
            settings1.importGCSpindleToggleLaser = false;
            settings1.importGCSSpeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.importGCSubEnable = false;
            settings1.importGCSubFirst = false;
            settings1.importGCSubPenUpDown = true;
            settings1.importGCSubroutine = "data\\scripts\\script_v1583_brush_refresh.nc";
            settings1.importGCTangentialAngle = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.importGCTangentialAngleDevi = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGCTangentialAxis = "A";
            settings1.importGCTangentialEnable = false;
            settings1.importGCTangentialRange = false;
            settings1.importGCTangentialShortening = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCTangentialShorteningEnable = false;
            settings1.importGCTangentialTurn = new decimal(new int[] {
            360,
            0,
            0,
            0});
            settings1.importGCTool = false;
            settings1.importGCToolChangeCode = "Z10 (move upwards)";
            settings1.importGCToolDefNr = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCToolDefNrUse = false;
            settings1.importGCToolListUse = false;
            settings1.importGCToolM0 = false;
            settings1.importGCToolUseRouter = false;
            settings1.importGCTTSSpeed = false;
            settings1.importGCTTXYFeed = false;
            settings1.importGCTTZAxis = false;
            settings1.importGCXYFeed = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            settings1.importGCZDown = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            settings1.importGCZEnable = true;
            settings1.importGCZFeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.importGCZIncEnable = false;
            settings1.importGCZIncNoZUp = false;
            settings1.importGCZIncrement = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGCZIncStartZero = false;
            settings1.importGCZPreventSpindle = false;
            settings1.importGCZUp = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGerberTypeEnable = true;
            settings1.importGerberTypeKnife = "knife";
            settings1.importGerberTypeM19 = "notch";
            settings1.importGerberTypePen = "pen";
            settings1.importGraphicAddFrameApplyRadius = false;
            settings1.importGraphicAddFrameDistance = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicAddFrameEnable = false;
            settings1.importGraphicAddFramePenColor = "black";
            settings1.importGraphicAddFramePenLayer = "Frame";
            settings1.importGraphicAddFramePenWidth = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.importGraphicClip = true;
            settings1.importGraphicClipAngle = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicClipAngleEnable = false;
            settings1.importGraphicClipEnable = false;
            settings1.importGraphicClipGCode = "(^2 G0 X#OFFX Y#OFFY)  (set offset via 2nd grbl)";
            settings1.importGraphicClipGetDimAuto = false;
            settings1.importGraphicClipOffsetApply = true;
            settings1.importGraphicClipOffsetX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicClipOffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicClipShowOrigPosition = false;
            settings1.importGraphicClipShowOrigPositionShiftTileProcessed = true;
            settings1.importGraphicClipSkipCode = false;
            settings1.importGraphicDevelopmentEnable = false;
            settings1.importGraphicDevelopmentFeedAfter = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGraphicDevelopmentFeedInvert = false;
            settings1.importGraphicDevelopmentFeedX = true;
            settings1.importGraphicDevelopmentNoCurve = false;
            settings1.importGraphicDevelopmentNotchDistance = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicDevelopmentNotchLift = true;
            settings1.importGraphicDevelopmentNotchWidth = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.importGraphicDevelopmentNotchZCut = new decimal(new int[] {
            15,
            0,
            0,
            -2147418112});
            settings1.importGraphicDevelopmentNotchZNotch = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            settings1.importGraphicDevelopmentToolAngle = new decimal(new int[] {
            90,
            0,
            0,
            0});
            settings1.importGraphicExtendPathEnable = false;
            settings1.importGraphicExtendPathValue = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGraphicFilterChoiceRemove = true;
            settings1.importGraphicFilterEnable = false;
            settings1.importGraphicFilterListKeep = "FFFFFF;BBBBBB";
            settings1.importGraphicFilterListRemove = "FFFFFF;BBBBBB";
            settings1.importGraphicHatchFillAngle = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.importGraphicHatchFillAngle2 = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.importGraphicHatchFillAngleInc = false;
            settings1.importGraphicHatchFillCross = false;
            settings1.importGraphicHatchFillDash = false;
            settings1.importGraphicHatchFillDeletePath = false;
            settings1.importGraphicHatchFillDistance = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGraphicHatchFillEnable = false;
            settings1.importGraphicHatchFillInset = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importGraphicHatchFillInsetEnable = true;
            settings1.importGraphicHatchFillInsetEnable2 = false;
            settings1.importGraphicHatchFillNoise = false;
            settings1.importGraphicHatchFillOffset = new decimal(new int[] {
            3,
            0,
            0,
            65536});
            settings1.importGraphicHatchFillOffsetInc = false;
            settings1.importGraphicLargestLast = true;
            settings1.importGraphicLeadInDistance = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicLeadInEnable = false;
            settings1.importGraphicLeadOutDistance = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicLeadOutEnable = false;
            settings1.importGraphicLeadTopZUp = false;
            settings1.importGraphicMultiplyGraphicsDimX = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGraphicMultiplyGraphicsDimY = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGraphicMultiplyGraphicsDistance = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicMultiplyGraphicsEnable = false;
            settings1.importGraphicNoiseAmplitude = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGraphicNoiseDensity = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importGraphicNoiseEnable = false;
            settings1.importGraphicOffsetLargestLast = true;
            settings1.importGraphicOffsetLargestRemove = false;
            settings1.importGraphicOffsetOrigin = true;
            settings1.importGraphicOffsetOriginX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicOffsetOriginY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicSortDimension = false;
            settings1.importGraphicSortDistance = true;
            settings1.importGraphicSortDistanceAllowRotate = true;
            settings1.importGraphicSortDistanceStart = 0;
            settings1.importGraphicTileAddOnX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importGraphicTileX = new decimal(new int[] {
            200,
            0,
            0,
            0});
            settings1.importGraphicTileY = new decimal(new int[] {
            200,
            0,
            0,
            0});
            settings1.importGraphicWireBenderAngleAbsolute = true;
            settings1.importGraphicWireBenderAngleAddOn = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.importGraphicWireBenderAxisAngle = "Y";
            settings1.importGraphicWireBenderAxisFeed = "X";
            settings1.importGraphicWireBenderCodeCut = "( cut wire )";
            settings1.importGraphicWireBenderCodePegOff = "( disable bending pegs )";
            settings1.importGraphicWireBenderCodePegOn = "( activate bending pegs )";
            settings1.importGraphicWireBenderDiameter = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGraphicWireBenderEnable = false;
            settings1.importGraphicWireBenderRadius = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importGroupItem = 1;
            settings1.importGroupObjects = false;
            settings1.importGroupSort = 1;
            settings1.importGroupSortInvert = false;
            settings1.importImage2DViewHideZero = true;
            settings1.importImageColorMode = true;
            settings1.importImageEngravingAngle = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importImageEngravingCross = false;
            settings1.importImageEngravingOneDirection = true;
            settings1.importImageEngravingTopDown = true;
            settings1.importImageGrayAsMode = 0;
            settings1.importImageGrayVectorize = false;
            settings1.importImageHeight = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.importImageKeepRatio = true;
            settings1.importImagePixelArt = false;
            settings1.importImagePixelArtDotSize = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importImagePixelArtDotsPerPixel = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importImagePixelArtDrawDot = true;
            settings1.importImagePixelArtDrawShapeCircle = true;
            settings1.importImagePixelArtDrawShapeScript = false;
            settings1.importImagePixelArtDrawShapeScriptText = "";
            settings1.importImagePixelArtGapSize = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importImagePixelArtShapeFill = false;
            settings1.importImagePixelArtShapePenDiameter = new decimal(new int[] {
            4,
            0,
            0,
            65536});
            settings1.importImageProcessingMode = 0;
            settings1.importImageReso = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            settings1.importImageResoApply = false;
            settings1.importImageResoY = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importImageSLasermodeEnd = false;
            settings1.importImageSLasermodeStart = false;
            settings1.importImageSLaserOnly = false;
            settings1.importImageSMax = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.importImageSMin = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.importImageSpecialCodeAfter = "G90 G0 Z5";
            settings1.importImageSpecialCodeBefore = "G90 G1 Z1 F1000";
            settings1.importImageSpecialCodeValue1 = "G91 A";
            settings1.importImageSpecialCodeValue2 = "F10000";
            settings1.importImageSpecialMax = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importImageSpecialMin = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            settings1.importImageWidth = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.importImageZMax = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.importImageZMin = new decimal(new int[] {
            2,
            0,
            0,
            -2147483648});
            settings1.importLineDashPattern = false;
            settings1.importLineDashPatternG0 = true;
            settings1.importLoggerSettings = ((uint)(0u));
            settings1.importMessageDelay = new decimal(new int[] {
            6,
            0,
            0,
            0});
            settings1.importPauseElement = false;
            settings1.importPausePenDown = false;
            settings1.importPDNDpi = new decimal(new int[] {
            96,
            0,
            0,
            0});
            settings1.importPDNLayerVisible = true;
            settings1.importPDNWidth = new decimal(new int[] {
            120,
            0,
            0,
            0});
            settings1.importPWMFromWidth = false;
            settings1.importRemoveShortMovesEnable = true;
            settings1.importRemoveShortMovesLimit = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.importRepeatCnt = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importRepeatComplete = true;
            settings1.importRepeatEnable = false;
            settings1.importRepeatEnableAll = false;
            settings1.importShowUseCaseDialog = true;
            settings1.importSVGAddComments = false;
            settings1.importSVGAddOnEnable = false;
            settings1.importSVGAddOnFile = "data\\examples\\addon_frame_stamp1.svg";
            settings1.importSVGAddOnPosition = 1;
            settings1.importSVGAddOnScale = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            settings1.importSVGApplyFill = false;
            settings1.importSVGCircleToDot = false;
            settings1.importSVGCircleToDotS = false;
            settings1.importSVGCircleToDotZ = false;
            settings1.importSVGDontPlot = false;
            settings1.importSVGDPI96 = true;
            settings1.importSVGMaxSize = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.importSVGMetaData = false;
            settings1.importSVGNodesOnly = false;
            settings1.importSVGPathExtend = false;
            settings1.importSVGPathNewFigure = false;
            settings1.importSVGRezise = false;
            settings1.importSVGUseElement = true;
            settings1.importUnitGCode = false;
            settings1.importUnitmm = true;
            settings1.importUseCaseInfo = "Information about current use case";
            settings1.importVectorizeAlgorithmPoTrace = true;
            settings1.importVectorizeDetectTransparency = true;
            settings1.importVectorizeDpiFromImage = true;
            settings1.importVectorizeFromClipboard = false;
            settings1.importVectorizeInvertResult = false;
            settings1.importVectorizeOptimize1 = false;
            settings1.importVectorizeOptimize2 = false;
            settings1.importVectorizeOptimize3 = false;
            settings1.importVectorizeOptimize4 = false;
            settings1.importVectorizePoTraceAlphamax = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.importVectorizePoTraceCurveoptimizing = true;
            settings1.importVectorizePoTraceOpttolerance = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.importVectorizePoTraceTurdsize = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.importVectorizeSetWidthOfImage = false;
            settings1.importVectorizeSmoothCycles = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.importVectorizeThreshold = new decimal(new int[] {
            128,
            0,
            0,
            0});
            settings1.importVectorizeTypeBmp = false;
            settings1.importVectorizeTypeGif = false;
            settings1.importVectorizeTypeJpg = false;
            settings1.importVectorizeTypePng = false;
            settings1.laserMotionDelay = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.laserMotionDelayPower = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.laserMotionX = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.laserMotionY = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.laserScanPowerFeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.laserScanPowerFrom = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.laserScanPowerStep = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.laserScanPowerTo = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.laserScanSpeedFrom = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            settings1.laserScanSpeedPower = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.laserScanSpeedStep = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.laserScanSpeedTo = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.laserScanZFeed = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            settings1.laserScanZPower = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.laserScanZRange = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.location2ndGRBLForm = new System.Drawing.Point(100, 200);
            settings1.locationCamForm = new System.Drawing.Point(-30, 0);
            settings1.locationCntrlCoordForm = new System.Drawing.Point(-30, 0);
            settings1.locationHeightForm = new System.Drawing.Point(-30, 0);
            settings1.locationImageForm = new System.Drawing.Point(-30, 0);
            settings1.locationJogCreatorForm = new System.Drawing.Point(250, 0);
            settings1.locationMForm = new System.Drawing.Point(-30, 0);
            settings1.locationProbingForm = new System.Drawing.Point(-30, 0);
            settings1.locationSerForm1 = new System.Drawing.Point(-30, 0);
            settings1.locationSerForm2 = new System.Drawing.Point(50, 50);
            settings1.locationSerForm3 = new System.Drawing.Point(250, 50);
            settings1.locationSerForm4 = new System.Drawing.Point(0, 0);
            settings1.locationSetForm = new System.Drawing.Point(-30, 0);
            settings1.locationShapeForm = new System.Drawing.Point(-30, 0);
            settings1.locationStreamForm = new System.Drawing.Point(-30, 0);
            settings1.locationTextForm = new System.Drawing.Point(-30, 0);
            settings1.locationWireCutterForm = new System.Drawing.Point(0, 0);
            settings1.machineLimitsAlarm = false;
            settings1.machineLimitsFix = false;
            settings1.machineLimitsHomeX = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            settings1.machineLimitsHomeY = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            settings1.machineLimitsHomeZ = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.machineLimitsRangeX = new decimal(new int[] {
            220,
            0,
            0,
            0});
            settings1.machineLimitsRangeY = new decimal(new int[] {
            220,
            0,
            0,
            0});
            settings1.machineLimitsRangeZ = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.machineLimitsShow = false;
            settings1.machineLoadDefaults = false;
            settings1.mainFormMovoToX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormMovoToY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormMovoToZ = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormSetCoordA = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormSetCoordX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormSetCoordY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormSetCoordZ = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.mainFormSize = new System.Drawing.Size(900, 680);
            settings1.mainFormSplitDistance = 296;
            settings1.mainFormWinState = System.Windows.Forms.FormWindowState.Normal;
            settings1.multipleLoadAllwaysClear = false;
            settings1.multipleLoadAllwaysLoad = true;
            settings1.multipleLoadByX = true;
            settings1.multipleLoadGap = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.multipleLoadLimitNo = true;
            settings1.multipleLoadNoX = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.multipleLoadNoY = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.notifierMailClientAdr = "smtp.gmail.com";
            settings1.notifierMailClientPass = "pass";
            settings1.notifierMailClientPort = new decimal(new int[] {
            587,
            0,
            0,
            0});
            settings1.notifierMailClientUser = "user";
            settings1.notifierMailEnable = false;
            settings1.notifierMailSendFrom = "from@gmail.com";
            settings1.notifierMailSendSubject = "GRBL-Plotter ";
            settings1.notifierMailSendTo = "to@gmail.com";
            settings1.notifierMessageFinish = "Streaming via GRBL-Plotter finished";
            settings1.notifierMessageProgress = "Progress";
            settings1.notifierMessageProgressEnable = true;
            settings1.notifierMessageProgressInterval = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.notifierMessageProgressTitle = true;
            settings1.notifierPushbulletChannel = "";
            settings1.notifierPushbulletEnable = false;
            settings1.notifierPushbulletToken = "";
            settings1.overrideFRBtm = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.overrideFRTop = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            settings1.overrideFRValue = 1000;
            settings1.overrideSSBtm = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.overrideSSTop = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            settings1.overrideSSValue = 5000;
            settings1.probingCoordG10 = true;
            settings1.probingEdgeCenter = false;
            settings1.probingEdgeZ = false;
            settings1.probingFeedXY = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.probingFeedZ = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.probingFiducial2ndChance = true;
            settings1.probingFiducialCodeEnd = "G54";
            settings1.probingFiducialCodeStart = "G59";
            settings1.probingFiducialOffsetX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.probingFiducialOffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.probingFiducialSkip1stMove = true;
            settings1.probingFinalX = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.probingFinalY = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.probingFinalZ = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.probingInvertLogic = false;
            settings1.probingOffsetX = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.probingOffsetY = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.probingOffsetZ = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.probingSaveX = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.probingSaveY = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.probingSaveZ = new decimal(new int[] {
            5,
            0,
            0,
            0});
            settings1.probingToolDiameter = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.probingTravelX = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.probingTravelY = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.probingTravelZ = new decimal(new int[] {
            30,
            0,
            0,
            0});
            settings1.probingWorkpieceDiameter = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.processCounter = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.processCounterDigits = new decimal(new int[] {
            6,
            0,
            0,
            0});
            settings1.processCounterFill = "0";
            settings1.processCounterFront = "Fixed front - ";
            settings1.processCounterRear = " - Fixed rear";
            settings1.processDataDelimeter = "; Semicolon";
            settings1.processDataIndex = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.processDataLastFile = "";
            settings1.processLastFile = "";
            settings1.processLocation = new System.Drawing.Point(0, 0);
            settings1.processOpenOnProgStart = false;
            settings1.processSize = new System.Drawing.Size(0, 0);
            settings1.processSplitDistance = 400;
            settings1.processTimerInterval = new decimal(new int[] {
            500,
            0,
            0,
            0});
            settings1.projectorColorBackground = System.Drawing.Color.Black;
            settings1.projectorColorDimension = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            settings1.projectorColorMarker = System.Drawing.Color.DeepPink;
            settings1.projectorColorPenDown = System.Drawing.Color.White;
            settings1.projectorColorPenUp = System.Drawing.Color.Green;
            settings1.projectorColorRuler = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            settings1.projectorColorTool = System.Drawing.Color.LightGray;
            settings1.projectorDisplayOffsetX = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.projectorDisplayOffsetY = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.projectorDisplayScale = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.projectorMonitorIndex = new decimal(new int[] {
            1,
            0,
            0,
            0});
            settings1.projectorShowDimension = false;
            settings1.projectorShowMarker = false;
            settings1.projectorShowPenUp = false;
            settings1.projectorShowRuler = true;
            settings1.projectorShowSetup = true;
            settings1.projectorShowTool = false;
            settings1.projectorWidthDimension = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.projectorWidthMarker = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.projectorWidthPenDown = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.projectorWidthPenUp = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.projectorWidthRuler = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            settings1.projectorWidthTool = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.resetRestoreWorkCoordinates = false;
            settings1.resetSendCode = "G10 L20 P0 X0 Y0 Z0";
            settings1.resetSendCodeEnable = false;
            settings1.rotarySubstitutionDiameter = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.rotarySubstitutionEnable = false;
            settings1.rotarySubstitutionScale = new decimal(new int[] {
            360,
            0,
            0,
            0});
            settings1.rotarySubstitutionSetupEnable = false;
            settings1.rotarySubstitutionSetupOff = "$100=1600;$110=1000;$120=50";
            settings1.rotarySubstitutionSetupOn = "$100=4.444;$110=10000;$120=1000";
            settings1.rotarySubstitutionX = true;
            settings1.selectionPropertyIncrement = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.serial3Ready = "ready";
            settings1.serial3Timeout = 10;
            settings1.serialBaud1 = "115200";
            settings1.serialBaud2 = "115200";
            settings1.serialBaud3 = "115200";
            settings1.serialBaudDIY = "115200";
            settings1.serialEthernetIP1 = "192.168.1.121";
            settings1.serialEthernetIP2 = "192.168.1.122";
            settings1.serialEthernetPort1 = 34000;
            settings1.serialEthernetPort2 = 34000;
            settings1.serialEthernetUse1 = false;
            settings1.serialEthernetUse2 = false;
            settings1.serialMinimize = true;
            settings1.serialPort1 = "COM1";
            settings1.serialPort2 = "COM2";
            settings1.serialPort3 = "COM3";
            settings1.serialPortDIY = "COM9";
            settings1.SettingsKey = "";
            settings1.setupPWMIncrement = new decimal(new int[] {
            10,
            0,
            0,
            0});
            settings1.sizeSerForm1 = new System.Drawing.Size(342, 480);
            settings1.sizeUseCase = new System.Drawing.Size(600, 500);
            settings1.tabletFitToCurve = true;
            settings1.tabletFormLocation = new System.Drawing.Point(0, 0);
            settings1.tabletFormSize = new System.Drawing.Size(500, 480);
            settings1.tabletPlotterMoveAir = false;
            settings1.tabletPlotterMoveMouse = false;
            settings1.tabletPointDistance = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            settings1.tabletShowTransparency = true;
            settings1.tabletSizePen = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.tabletSizeX = new decimal(new int[] {
            200,
            0,
            0,
            0});
            settings1.tabletSizeY = new decimal(new int[] {
            100,
            0,
            0,
            0});
            settings1.toolTableLastLoaded = "empty";
            settings1.toolTableOffsetA = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.toolTableOffsetX = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.toolTableOffsetY = new decimal(new int[] {
            120,
            0,
            0,
            0});
            settings1.toolTableOffsetZ = new decimal(new int[] {
            0,
            0,
            0,
            0});
            settings1.toolTableOriginal = false;
            settings1.useCaseLastLoaded = "?";
            settings1.UserControlFlowControlIsLarge = true;
            settings1.UserControlJogControlAllIsLarge = true;
            settings1.UserControlJogControlAutomaticUnfold = false;
            settings1.UserControlJogControlShowArrow = true;
            settings1.UserControlJogControlShowButtons = false;
            settings1.UserControlJogControlShowLabel = true;
            settings1.UserControlJogControlSize = 100;
            settings1.UserControlMoveToGraphicAutomaticUnfold = false;
            settings1.UserControlMoveToGraphicCenterX = true;
            settings1.UserControlMoveToGraphicCenterY = true;
            settings1.UserControlMoveToGraphicFeed = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            settings1.UserControlMoveToGraphicIsLarge = true;
            settings1.UserControlMoveToGraphicSize = 100;
            settings1.UserControlMoveToZeroAutomaticUnfold = false;
            settings1.UserControlMoveToZeroIsLarge = true;
            settings1.UserControlOffsetIsLarge = true;
            settings1.UserControlOverrideIsLarge = false;
            settings1.UserControlOverrideShow1 = true;
            settings1.UserControlOverrideShow2 = true;
            settings1.UserControlOverrideShow3 = true;
            settings1.UserControlOverrideShow4 = false;
            settings1.UserControlSetCoordinateAutomaticUnfold = false;
            settings1.UserControlSetCoordinateIsLarge = false;
            settings1.wirecutterArrayX = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.wirecutterArrayY = new decimal(new int[] {
            3,
            0,
            0,
            0});
            settings1.wirecutterGapX = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.wirecutterGapY = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.wirecutterRingDiameterIn = new decimal(new int[] {
            20,
            0,
            0,
            0});
            settings1.wirecutterRingDiameterOut = new decimal(new int[] {
            50,
            0,
            0,
            0});
            settings1.wirecutterRingSections = new decimal(new int[] {
            2,
            0,
            0,
            0});
            settings1.wirecutterToolDiameter = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.CbAddGraphic.DataBindings.Add(new System.Windows.Forms.Binding("Checked", settings1, "fromFormInsertEnable", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.CbAddGraphic.Name = "CbAddGraphic";
            this.toolTip1.SetToolTip(this.CbAddGraphic, resources.GetString("CbAddGraphic.ToolTip"));
            this.CbAddGraphic.UseVisualStyleBackColor = true;
            this.CbAddGraphic.CheckedChanged += new System.EventHandler(this.CbAddGraphic_CheckedChanged);
            // 
            // ucFlowControl
            // 
            resources.ApplyResources(this.ucFlowControl, "ucFlowControl");
            this.ucFlowControl.Name = "ucFlowControl";
            // 
            // ucSetOffset
            // 
            resources.ApplyResources(this.ucSetOffset, "ucSetOffset");
            this.ucSetOffset.Name = "ucSetOffset";
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
            this.tLPRechtsUntenRechts.Controls.Add(this.ucMoveToGraphic, 0, 1);
            this.tLPRechtsUntenRechts.Controls.Add(this.ucMoveToZero, 0, 3);
            this.tLPRechtsUntenRechts.Controls.Add(this.ucJogControlAll, 0, 4);
            this.tLPRechtsUntenRechts.Controls.Add(this.tC_RouterPlotterLaser2, 0, 0);
            this.tLPRechtsUntenRechts.Controls.Add(this.ucSetCoordinateSystem, 0, 2);
            this.tLPRechtsUntenRechts.Name = "tLPRechtsUntenRechts";
            // 
            // ucMoveToGraphic
            // 
            resources.ApplyResources(this.ucMoveToGraphic, "ucMoveToGraphic");
            this.ucMoveToGraphic.Name = "ucMoveToGraphic";
            // 
            // ucMoveToZero
            // 
            resources.ApplyResources(this.ucMoveToZero, "ucMoveToZero");
            this.ucMoveToZero.Name = "ucMoveToZero";
            // 
            // ucJogControlAll
            // 
            resources.ApplyResources(this.ucJogControlAll, "ucJogControlAll");
            this.ucJogControlAll.Name = "ucJogControlAll";
            // 
            // tC_RouterPlotterLaser2
            // 
            this.tC_RouterPlotterLaser2.Controls.Add(this.tabPageLaser);
            this.tC_RouterPlotterLaser2.Controls.Add(this.tabPagePlotter);
            this.tC_RouterPlotterLaser2.Controls.Add(this.tabPage5);
            resources.ApplyResources(this.tC_RouterPlotterLaser2, "tC_RouterPlotterLaser2");
            this.tC_RouterPlotterLaser2.Name = "tC_RouterPlotterLaser2";
            this.tC_RouterPlotterLaser2.SelectedIndex = 0;
            this.tC_RouterPlotterLaser2.SelectedIndexChanged += new System.EventHandler(this.TC_RouterPlotterLaser2_SelectedIndexChanged);
            // 
            // tabPageLaser
            // 
            this.tabPageLaser.Controls.Add(this.ucDeviceLaser2);
            resources.ApplyResources(this.tabPageLaser, "tabPageLaser");
            this.tabPageLaser.Name = "tabPageLaser";
            this.tabPageLaser.UseVisualStyleBackColor = true;
            // 
            // ucDeviceLaser2
            // 
            resources.ApplyResources(this.ucDeviceLaser2, "ucDeviceLaser2");
            this.ucDeviceLaser2.Name = "ucDeviceLaser2";
            // 
            // tabPagePlotter
            // 
            this.tabPagePlotter.Controls.Add(this.ucDevicePlotter2);
            resources.ApplyResources(this.tabPagePlotter, "tabPagePlotter");
            this.tabPagePlotter.Name = "tabPagePlotter";
            this.tabPagePlotter.UseVisualStyleBackColor = true;
            // 
            // ucDevicePlotter2
            // 
            resources.ApplyResources(this.ucDevicePlotter2, "ucDevicePlotter2");
            this.ucDevicePlotter2.Name = "ucDevicePlotter2";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.ucDeviceRouter2);
            resources.ApplyResources(this.tabPage5, "tabPage5");
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // ucDeviceRouter2
            // 
            resources.ApplyResources(this.ucDeviceRouter2, "ucDeviceRouter2");
            this.ucDeviceRouter2.Name = "ucDeviceRouter2";
            // 
            // ucSetCoordinateSystem
            // 
            resources.ApplyResources(this.ucSetCoordinateSystem, "ucSetCoordinateSystem");
            this.ucSetCoordinateSystem.Name = "ucSetCoordinateSystem";
            // 
            // tLPMitteUnten
            // 
            resources.ApplyResources(this.tLPMitteUnten, "tLPMitteUnten");
            this.tLPMitteUnten.Controls.Add(this.pictureBox1, 0, 0);
            this.tLPMitteUnten.Controls.Add(this.CbAddGraphic, 0, 2);
            this.tLPMitteUnten.Controls.Add(this.tBURL, 0, 3);
            this.tLPMitteUnten.Controls.Add(this.tLPMitteUnten1Zeile, 0, 1);
            this.tLPMitteUnten.Name = "tLPMitteUnten";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.ContextMenuStrip = this.cmsPictureBox;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.SizeChanged += new System.EventHandler(this.PictureBox1_SizeChanged);
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.PictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDown);
            this.pictureBox1.MouseLeave += new System.EventHandler(this.PictureBox1_MouseLeave);
            this.pictureBox1.MouseHover += new System.EventHandler(this.PictureBox1_MouseHover);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseWheel);
            // 
            // tLPMitteUnten1Zeile
            // 
            resources.ApplyResources(this.tLPMitteUnten1Zeile, "tLPMitteUnten1Zeile");
            this.tLPMitteUnten1Zeile.Controls.Add(this.Rb2DViewMode1, 0, 0);
            this.tLPMitteUnten1Zeile.Controls.Add(this.Rb2DViewMode2, 1, 0);
            this.tLPMitteUnten1Zeile.Controls.Add(this.Rb2DViewMode3, 2, 0);
            this.tLPMitteUnten1Zeile.Name = "tLPMitteUnten1Zeile";
            // 
            // tLPRechtsOben
            // 
            resources.ApplyResources(this.tLPRechtsOben, "tLPRechtsOben");
            this.tLPRechtsOben.Controls.Add(this.ucdro, 0, 0);
            this.tLPRechtsOben.Controls.Add(this.splitContainer2, 1, 0);
            this.tLPRechtsOben.Name = "tLPRechtsOben";
            // 
            // ucdro
            // 
            resources.ApplyResources(this.ucdro, "ucdro");
            this.ucdro.MainWidth = 900;
            this.ucdro.Name = "ucdro";
            this.ucdro.ZeroCommand = "G10L20P0";
            this.ucdro.ZeroString = "0.000";
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.GbCustomButtons);
            this.splitContainer2.Panel1.Controls.Add(this.ucToolList);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tC_RouterPlotterLaser);
            // 
            // GbCustomButtons
            // 
            this.GbCustomButtons.Controls.Add(this.tLPCustomButton1);
            resources.ApplyResources(this.GbCustomButtons, "GbCustomButtons");
            this.GbCustomButtons.Name = "GbCustomButtons";
            this.GbCustomButtons.TabStop = false;
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
            this.btnCustom1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom2
            // 
            resources.ApplyResources(this.btnCustom2, "btnCustom2");
            this.btnCustom2.Name = "btnCustom2";
            this.btnCustom2.UseVisualStyleBackColor = true;
            this.btnCustom2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom3
            // 
            resources.ApplyResources(this.btnCustom3, "btnCustom3");
            this.btnCustom3.Name = "btnCustom3";
            this.btnCustom3.UseVisualStyleBackColor = true;
            this.btnCustom3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom4
            // 
            resources.ApplyResources(this.btnCustom4, "btnCustom4");
            this.btnCustom4.Name = "btnCustom4";
            this.btnCustom4.UseVisualStyleBackColor = true;
            this.btnCustom4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom5
            // 
            resources.ApplyResources(this.btnCustom5, "btnCustom5");
            this.btnCustom5.Name = "btnCustom5";
            this.btnCustom5.UseVisualStyleBackColor = true;
            this.btnCustom5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom6
            // 
            resources.ApplyResources(this.btnCustom6, "btnCustom6");
            this.btnCustom6.Name = "btnCustom6";
            this.btnCustom6.UseVisualStyleBackColor = true;
            this.btnCustom6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom7
            // 
            resources.ApplyResources(this.btnCustom7, "btnCustom7");
            this.btnCustom7.Name = "btnCustom7";
            this.btnCustom7.UseVisualStyleBackColor = true;
            this.btnCustom7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom8
            // 
            resources.ApplyResources(this.btnCustom8, "btnCustom8");
            this.btnCustom8.Name = "btnCustom8";
            this.btnCustom8.UseVisualStyleBackColor = true;
            this.btnCustom8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom9
            // 
            resources.ApplyResources(this.btnCustom9, "btnCustom9");
            this.btnCustom9.Name = "btnCustom9";
            this.btnCustom9.UseVisualStyleBackColor = true;
            this.btnCustom9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom10
            // 
            resources.ApplyResources(this.btnCustom10, "btnCustom10");
            this.btnCustom10.Name = "btnCustom10";
            this.btnCustom10.UseVisualStyleBackColor = true;
            this.btnCustom10.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom11
            // 
            resources.ApplyResources(this.btnCustom11, "btnCustom11");
            this.btnCustom11.Name = "btnCustom11";
            this.btnCustom11.UseVisualStyleBackColor = true;
            this.btnCustom11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom12
            // 
            resources.ApplyResources(this.btnCustom12, "btnCustom12");
            this.btnCustom12.Name = "btnCustom12";
            this.btnCustom12.UseVisualStyleBackColor = true;
            this.btnCustom12.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom13
            // 
            resources.ApplyResources(this.btnCustom13, "btnCustom13");
            this.btnCustom13.Name = "btnCustom13";
            this.btnCustom13.UseVisualStyleBackColor = true;
            this.btnCustom13.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom14
            // 
            resources.ApplyResources(this.btnCustom14, "btnCustom14");
            this.btnCustom14.Name = "btnCustom14";
            this.btnCustom14.UseVisualStyleBackColor = true;
            this.btnCustom14.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom15
            // 
            resources.ApplyResources(this.btnCustom15, "btnCustom15");
            this.btnCustom15.Name = "btnCustom15";
            this.btnCustom15.UseVisualStyleBackColor = true;
            this.btnCustom15.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // btnCustom16
            // 
            resources.ApplyResources(this.btnCustom16, "btnCustom16");
            this.btnCustom16.Name = "btnCustom16";
            this.btnCustom16.UseVisualStyleBackColor = true;
            this.btnCustom16.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BtnCustomButton_Click);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // ucToolList
            // 
            resources.ApplyResources(this.ucToolList, "ucToolList");
            this.ucToolList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ucToolList.Name = "ucToolList";
            // 
            // tC_RouterPlotterLaser
            // 
            this.tC_RouterPlotterLaser.Controls.Add(this.tabPage3);
            this.tC_RouterPlotterLaser.Controls.Add(this.tabPage2);
            this.tC_RouterPlotterLaser.Controls.Add(this.tabPage1);
            this.tC_RouterPlotterLaser.Controls.Add(this.tabPage4);
            resources.ApplyResources(this.tC_RouterPlotterLaser, "tC_RouterPlotterLaser");
            this.tC_RouterPlotterLaser.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tC_RouterPlotterLaser.Name = "tC_RouterPlotterLaser";
            this.tC_RouterPlotterLaser.SelectedIndex = 0;
            this.tC_RouterPlotterLaser.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.Tc_RouterPlotterLaser_DrawItem);
            this.tC_RouterPlotterLaser.SelectedIndexChanged += new System.EventHandler(this.TC_RouterPlotterLaser_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.ucDeviceLaser);
            resources.ApplyResources(this.tabPage3, "tabPage3");
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // ucDeviceLaser
            // 
            this.ucDeviceLaser.AllowDrop = true;
            resources.ApplyResources(this.ucDeviceLaser, "ucDeviceLaser");
            this.ucDeviceLaser.LaserMode = false;
            this.ucDeviceLaser.Name = "ucDeviceLaser";
            this.ucDeviceLaser.SpindleMax = 1000;
            this.ucDeviceLaser.SpindleMin = 0;
            this.ucDeviceLaser.SpindleSet = "0";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.ucDevicePlotter);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // ucDevicePlotter
            // 
            resources.ApplyResources(this.ucDevicePlotter, "ucDevicePlotter");
            this.ucDevicePlotter.Name = "ucDevicePlotter";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ucDeviceRouter);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ucDeviceRouter
            // 
            resources.ApplyResources(this.ucDeviceRouter, "ucDeviceRouter");
            this.ucDeviceRouter.Name = "ucDeviceRouter";
            this.ucDeviceRouter.SpindleMax = 1000;
            this.ucDeviceRouter.SpindleMin = 0;
            this.ucDeviceRouter.SpindleSet = "0";
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.IndividualSettings);
            resources.ApplyResources(this.tabPage4, "tabPage4");
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // IndividualSettings
            // 
            resources.ApplyResources(this.IndividualSettings, "IndividualSettings");
            this.IndividualSettings.Name = "IndividualSettings";
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
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
            this.ResizeEnd += new System.EventHandler(this.MainForm_Resize);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.MainForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.MainForm_DragEnter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.MainForm_PreviewKeyDown);
            this.cmsFCTB.ResumeLayout(false);
            this.cmsPictureBox.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.cmsPictureBox2.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tLPLinks.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fCTBCode)).EndInit();
            this.tLPRechts.ResumeLayout(false);
            this.tLPRechtsUnten.ResumeLayout(false);
            this.tLPRechtsUnten.PerformLayout();
            this.tLPRechtsUntenRechts.ResumeLayout(false);
            this.tC_RouterPlotterLaser2.ResumeLayout(false);
            this.tabPageLaser.ResumeLayout(false);
            this.tabPagePlotter.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tLPMitteUnten.ResumeLayout(false);
            this.tLPMitteUnten.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tLPMitteUnten1Zeile.ResumeLayout(false);
            this.tLPMitteUnten1Zeile.PerformLayout();
            this.tLPRechtsOben.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.GbCustomButtons.ResumeLayout(false);
            this.tLPCustomButton1.ResumeLayout(false);
            this.tLPCustomButton2.ResumeLayout(false);
            this.tC_RouterPlotterLaser.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Timer MainTimer;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox GbCustomButtons;
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
        private System.Windows.Forms.ToolStripMenuItem LanguageToolStripMenuItem3;
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem rotaryDimaeterToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_rotary_diameter;
        private System.Windows.Forms.ToolStripMenuItem setupToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem ersetzteG23DurchLinienToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem heightMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxResetZooming;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxPasteFromClipboard;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem cmsCommentOut;
        private System.Windows.Forms.Timer gamePadTimer;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMoveToMarkedPosition;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxReloadFile;
        private System.Windows.Forms.ToolStripMenuItem startStreamingAtLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStrip_tb_StreamLine;
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
        private System.Windows.Forms.ToolStripMenuItem mirrorRotaryToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem convertZToSspindleSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rotate180ToolStripMenuItem;
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
        private System.Windows.Forms.Timer simulationTimer;
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
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxRotateSelectedPath;
        private System.Windows.Forms.ToolStripMenuItem czechToolStripMenuItem;
        private System.Windows.Forms.TabControl tC_RouterPlotterLaser;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.ToolStripMenuItem showFormsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectorToolStripMenuItem;
        private System.Windows.Forms.CheckBox CbAddGraphic;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxDuplicatePath;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxShowProperties;
        private System.Windows.Forms.ToolStripMenuItem toggleBlockExpansionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grblSetupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem italianoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem processAutomationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem türkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PolishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxClearWorkspace;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxReloadFile2;
        private System.Windows.Forms.ToolStripMenuItem convertToPolarCoordinatesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem applyLastTransformToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireCutterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useOrigin;
        private System.Windows.Forms.TableLayoutPanel tLPMitteUnten1Zeile;
        private System.Windows.Forms.RadioButton Rb2DViewMode1;
        private System.Windows.Forms.RadioButton Rb2DViewMode2;
        private System.Windows.Forms.RadioButton Rb2DViewMode3;
        private System.Windows.Forms.ContextMenuStrip cmsPictureBox2;
        private System.Windows.Forms.ToolStripMenuItem cms2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem moveBetweenLastPositionsToolStripMenuItem;
        private System.Windows.Forms.Timer moveTimer;
        private System.Windows.Forms.ToolStripMenuItem cmsPicBoxMoveGraphicsOriginTo00;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem offsetGraphicsMarkerPositionTolastToolPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem directControlToolStripMenuItem;
        private UserControls.UCStreaming ucStreaming;
        private UserControls.UCOverrides ucOverrides;
        private UserControls.UCFlowControl ucFlowControl;
        private UserControls.UCSetOffset ucSetOffset;
        private UserControls.UCDRO ucdro;
        private System.Windows.Forms.TabPage tabPage4;
        private UserControls.UCMoveToGraphic ucMoveToGraphic;
        private UserControls.UCMoveToZero ucMoveToZero;
        private UserControls.UCJogControlAll ucJogControlAll;
        private UserControls.UCDeviceLaser2 ucDeviceLaser2;
        private UserControls.UCDevicePlotter ucDevicePlotter;
        private System.Windows.Forms.TabControl tC_RouterPlotterLaser2;
        private System.Windows.Forms.TabPage tabPageLaser;
        private System.Windows.Forms.TabPage tabPagePlotter;
        private UserControls.UCDevicePlotter2 ucDevicePlotter2;
        private UserControls.UCToolList ucToolList;
        private UserControls.UCDeviceLaser ucDeviceLaser;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TabPage tabPage5;
        private UserControls.UCDeviceRouter ucDeviceRouter;
        private UserControls.UCDeviceRouter2 ucDeviceRouter2;
        private System.Windows.Forms.Label IndividualSettings;
        private UserControls.UCSetCoordinateSystem ucSetCoordinateSystem;
    }
}

