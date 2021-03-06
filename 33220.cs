using System;
using System.Collections.Generic;
using System.Text;
using Ivi.Visa.Interop;
//'' """""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""
//''  © Agilent Technologies, Inc. 2013
//''
//'' You have a royalty-free right to use, modify, reproduce and distribute
//'' the Sample Application Files (and/or any modified version) in any way
//'' you find useful, provided that you agree that Agilent Technologies has no
//'' warranty,  obligations or liability for any Sample Application Files.
//''
//'' Agilent Technologies provides programming examples for illustration only,
//'' This sample program assumes that you are familiar with the programming
//'' language being demonstrated and the tools used to create and debug
//'' procedures. Agilent Technologies support engineers can help explain the
//'' functionality of Agilent Technologies software components and associated
//'' commands, but they will not modify these samples to provide added
//'' functionality or construct procedures to meet your specific needs.
//'' """""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""""

namespace CsharpExample
{
    class FunctionsExample
    {
        static void Main(string[] args)
        {

            FunctionsExample DmmClass = new FunctionsExample(); //Create an instance of this class so we can call functions from Main
                //For more information on getting started using VISA COM I/O operations see the app note located at:
                //http://cp.literature.agilent.com/litweb/pdf/5989-6338EN.pdf
                Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager(); //Open up a new resource manager
                Ivi.Visa.Interop.FormattedIO488 myDmm = new Ivi.Visa.Interop.FormattedIO488(); //Open a new Formatted IO 488 session 
            /*
                AGILENT_33220A = "USB0::0x0957::0x0407::MY44021621::0::INSTR"
                AGILENT_33522A = "USB0::0x0957::0x2307::MY50003961::0::INSTR"
            */
            try
            {                
                string AGILENT_33220A = "USB0::0x0957::0x0407::MY44021621::0::INSTR";
                string AGILENT_33522A = "USB0::0x0957::0x2307::MY50003961::0::INSTR";

                myDmm.IO = (IMessage)rm.Open(AGILENT_33220A, AccessMode.NO_LOCK, 2000, ""); //Open up a handle to the DMM with a 2 second timeout
                myDmm.IO.Timeout = 3000; //You can also set your timeout by doing this command, sets to 3 seconds
                
                //First start off with a reset state
                myDmm.IO.Clear(); //Send a device clear first to stop any measurements in process
                myDmm.WriteString("*RST", true); //Reset the device
                myDmm.WriteString("*IDN?", true); //Get the IDN string                
                string IDN = myDmm.ReadString();
                Console.WriteLine(IDN); //report the DMM's identity
                myDmm.WriteString(":OUTP:STAT 0", true);                    // Disable Output
                myDmm.WriteString(":SOUR:FREQ:CW 1.1512 MHZ", true);
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
                myDmm.WriteString(":SOUR:VOLT:LEV:IMM:AMPL 0.1 VPP", true);
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
                myDmm.WriteString(":SOUR:BURS:PHAS 0", true);
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
                System.Threading.Thread.Sleep(5000);
                myDmm.WriteString(":SOUR:BURS:NCYC 50000", true);
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
                myDmm.WriteString(":SOUR:BURS:INT:PER 0.1 S", true);
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors                
                myDmm.WriteString(":SOUR:FREQ?", true);
                string FreqRes = myDmm.ReadString();
                myDmm.WriteString(":SOUR:BURS:INT:PER?", true);
                string PeriodRes = myDmm.ReadString();
                myDmm.WriteString(":SOUR:VOLT:LEV:IMM:AMPL?", true);
                string AmplitudeRes = myDmm.ReadString();
                myDmm.WriteString(":SOUR:BURS:PHAS?", true);
                string PhasResult = myDmm.ReadString();
                myDmm.WriteString(":SOUR:BURS:NCYC?", true);
                string NumCycRes = myDmm.ReadString();
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
                Console.WriteLine("Current Values are:" + "Period:" + PeriodRes + "Frequency:" + FreqRes + "Amplitude" + AmplitudeRes + "Phase" + PhasResult + "Cycles" + NumCycRes); 
				myDmm.WriteString(":SOUR:BURS:STAT 1", true);
				DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
				myDmm.WriteString(":OUTP:STAT 1", true);                    // Enable Output
                DmmClass.CheckDMMError(myDmm); //Check if the DMM has any errors
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured: " + e.Message);
            }
            finally
            {
                //Close out your resources
                try { myDmm.IO.Close(); }
                catch{}
                try{ System.Runtime.InteropServices.Marshal.ReleaseComObject(myDmm);}
                catch {}
                try{
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rm);
                }
                catch {}
				/*
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
				*/
			}
        }
        public void CheckDMMError(FormattedIO488 myDmm)
        {

            myDmm.WriteString("SYST:ERR?", true);
            string errStr = myDmm.ReadString();
            if (errStr.Contains("No error")) //If no error, then return
                return;
            //If there is an error, read out all of the errors and return them in an exception
            else
            {
                string errStr2 = "";                
                do
                {
                    myDmm.WriteString("SYST:ERR?", true);
                    errStr2 = myDmm.ReadString();
                    if (!errStr2.Contains("No error")) errStr = errStr + "\n" + errStr2;

                } while (!errStr2.Contains("No error"));                
                throw new Exception("Exception: Encountered system error(s)\n" + errStr);
            }
        }
    }
}
