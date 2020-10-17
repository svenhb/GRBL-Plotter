//    Name:        _script_functions.js
//    Version:     2020-09-01
//    Function:    support frequently needed functions

// 2020-09-01 add work/machine coordinates
// 2020-03-08 initial version with GRBL-Plotter Vers 1.3.4.0
		
//Enabled Active X in the broswer security settings.
//In IE9:
//a) Go to Tools ->Internet Options
//b) Select security tab
//c) Click on Trusted Sites (or Local Intranet depending on whether your site is trusted or not)
//d) Click on Custom Level
//e) Ensure that "Initialize and script active x controls is not marked safe for scripting" is enabled.		


// Settings from GRBL-Plotter - read from registry
var GRBL_Plt_decimals = 3;
var GRBL_Plt_header = "(not updated)";
var GRBL_Plt_footer = "(not updated)";
var GRBL_Plt_feedrate_xy = 999;			// ugly default values
var GRBL_Plt_spindle_speed = 999;		// to check if registry read works
var GRBL_Plt_spindle_delay = 1;
var GRBL_Plt_feedrate_z = 99;
var GRBL_Plt_z_save  	= 2.1;
var GRBL_Plt_z_engrave  = -2.1;

var GRBL_Plt_work_x = 0;
var GRBL_Plt_work_y = 0;
var GRBL_Plt_work_z = 0;
var GRBL_Plt_work_a = 0;
var GRBL_Plt_work_b = 0;
var GRBL_Plt_work_c = 0;
var GRBL_Plt_machine_x = 0;
var GRBL_Plt_machine_y = 0;
var GRBL_Plt_machine_z = 0;
var GRBL_Plt_machine_a = 0;
var GRBL_Plt_machine_b = 0;
var GRBL_Plt_machine_c = 0;


var WshShell = new ActiveXObject("WScript.Shell");
function read_GRBL_Plotter_Settings()
{	var reg_key="HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\GCodeSettings\\";
	try 
	{	GRBL_Plt_decimals 		= WshShell.RegRead(reg_key+"DecimalPlaces");	// from [Setup - G-Code generation]
		GRBL_Plt_header   		= WshShell.RegRead(reg_key+"GcodeHeader");		// from [Setup - G-Code generation]
		GRBL_Plt_footer   		= WshShell.RegRead(reg_key+"GcodeFooter");		// from [Setup - G-Code generation]
		GRBL_Plt_feedrate_xy   	= WshShell.RegRead(reg_key+"XY_FeedRate");		// from [Setup - G-Code generation]
		GRBL_Plt_spindle_speed 	= WshShell.RegRead(reg_key+"SpindleSpeed");		// from [Setup - G-Code generation]
		GRBL_Plt_spindle_delay 	= WshShell.RegRead(reg_key+"SpindleDelay");		// from [Setup - G-Code generation]
		GRBL_Plt_feedrate_z    	= WshShell.RegRead(reg_key+"Z_FeedRate");		// from [Setup - G-Code generation]
		GRBL_Plt_z_save        	= WshShell.RegRead(reg_key+"Z_Save");			// from [Setup - G-Code generation]
		GRBL_Plt_z_engrave     	= WshShell.RegRead(reg_key+"Z_Engrave");		// from [Setup - G-Code generation]
	}
    catch (err) {}
	
	var reg_key="HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\Position\\";
	try 
	{	GRBL_Plt_work_x 	= WshShell.RegRead(reg_key+"Work_X");
		GRBL_Plt_work_y 	= WshShell.RegRead(reg_key+"Work_Y");
		GRBL_Plt_work_z 	= WshShell.RegRead(reg_key+"Work_Z");
		GRBL_Plt_work_a 	= WshShell.RegRead(reg_key+"Work_A");
		GRBL_Plt_work_b 	= WshShell.RegRead(reg_key+"Work_B");
		GRBL_Plt_work_c 	= WshShell.RegRead(reg_key+"Work_C");
		GRBL_Plt_machine_x 	= WshShell.RegRead(reg_key+"Machine_X");
		GRBL_Plt_machine_y 	= WshShell.RegRead(reg_key+"Machine_Y");
		GRBL_Plt_machine_z 	= WshShell.RegRead(reg_key+"Machine_Z");
		GRBL_Plt_machine_a 	= WshShell.RegRead(reg_key+"Machine_A");
		GRBL_Plt_machine_b 	= WshShell.RegRead(reg_key+"Machine_B");
		GRBL_Plt_machine_c 	= WshShell.RegRead(reg_key+"Machine_C");
	}
    catch (err) {}
}

function set_GRBL_Plotter_Update()
{	var reg_key="HKEY_CURRENT_USER\\SOFTWARE\\GRBL-Plotter\\";
	try 
	{	GRBL_Plt_decimals    = WshShell.RegWrite(reg_key + "Update",1,"REG_DWORD");	}
    catch (err) {}
}

function copyTextToClipboard(text) 	// https://stackoverflow.com/questions/400212/how-do-i-copy-to-the-clipboard-in-javascript
{	if (!navigator.clipboard) 
	{	fallbackCopyTextToClipboard(text);
		return;
	}
	navigator.clipboard.writeText(text).then
	(	function() 	  { html_info.innerText += 'Async: Copying to clipboard was successful!';}, 
		function(err) { html_info.innerText += 'Async: Could not copy text: ' + err;}
	);
}

function fallbackCopyTextToClipboard(text) {
	var textArea = document.createElement("textarea");
	textArea.value = text;

	// Avoid scrolling to bottom
	textArea.style.top = "0";
	textArea.style.left = "0";
	textArea.style.position = "fixed";

	document.body.appendChild(textArea);
	textArea.focus();
	textArea.select();

	try {
		var successful = document.execCommand('copy');
		var msg = successful ? 'successful' : 'unsuccessful';
        html_info.innerText += 'Fallback: Copying text command was ' + msg;
		} catch (err) {
        html_info.innerText += 'Fallback: Oops, unable to copy ' + err;
	}
  document.body.removeChild(textArea);
}

