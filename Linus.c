[Back]

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <libusb-1.0/libusb.h>


//=========================================================================
// This program detects usb and print out their details
//=========================================================================


int main (int argc, char *argv)
{
   libusb_device                    **devList = NULL;
   libusb_device                    *devPtr = NULL;
   libusb_device_handle             *devHandle = NULL;
   libusb_context                   *ctx = NULL;            //a libusb session
   struct libusb_device_descriptor  devDesc;


   unsigned char              strDesc[256];
   ssize_t                    numUsbDevs = 0;      // pre-initialized scalars
   ssize_t                    idx = 0;
   int                        retVal = 0;


   //========================================================================
   // test out the libusb functions
   //========================================================================


   printf ("*************************\n USB Detection Program:\n*************************\n");
   retVal = libusb_init (&ctx);
   
   if(retVal < 0) {
        printf ("%d",retVal," Init Error "); //there was an error
            return 1;
              }


   //========================================================================
   // Get the list of USB devices visible to the system.
   //========================================================================


   numUsbDevs = libusb_get_device_list (ctx, &devList);


   //========================================================================
   // Loop through the list, looking for the device 
   //========================================================================


   while (idx < numUsbDevs)
   {
      printf ("\n\n[%d]\n", idx+1);


      //=====================================================================
      // Get next device pointer out of the list, use it to open the device.
      //=====================================================================


      devPtr = devList[idx];


      retVal = libusb_open (devPtr, &devHandle);
      if (retVal != LIBUSB_SUCCESS)
         break;


      //=====================================================================
      // Get the device descriptor for this device.
      //=====================================================================


      retVal = libusb_get_device_descriptor (devPtr, &devDesc);
      if (retVal != LIBUSB_SUCCESS)
         break;


      //=====================================================================
      // Get the string associated with iManufacturer index.
      //=====================================================================


     printf ("iManufacturer = %d", devDesc.iManufacturer);
      if (devDesc.iManufacturer > 0)
      {
         retVal = libusb_get_string_descriptor_ascii
                  (devHandle, devDesc.iManufacturer, strDesc, 256);
         if (retVal < 0)
            break;


         printf ("     string = %s",  strDesc);
      }


      //========================================================================
      // Get string associated with iProduct index.
      //========================================================================


      printf ("  \niProduct      = %d", devDesc.iProduct);
      if (devDesc.iProduct > 0)
      {
         retVal = libusb_get_string_descriptor_ascii
                  (devHandle, devDesc.iProduct, strDesc, 256);
         if (retVal < 0)
            break;


         printf ("     string = %s", strDesc);
      }


      //==================================================================
      // Get string associated with iSerialNumber index.
      //==================================================================


      printf ("  \niSerialNumber = %d", devDesc.iSerialNumber);
      if (devDesc.iSerialNumber > 0)
      {
         retVal = libusb_get_string_descriptor_ascii
                  (devHandle, devDesc.iSerialNumber, strDesc, 256);
         if (retVal < 0)
            break;


         printf ("     string = %s", strDesc);
      }


      //==================================================================
      // Print product id and Vendor id
      //==================================================================
  
        printf ("  \nProductid     = %d", devDesc.idProduct);
        printf ("  \nVendorid      = %d", devDesc.idVendor);
      
      //========================================================================
      // Close and try next one.
      //========================================================================


      libusb_close (devHandle);
      devHandle = NULL;
      idx++;


      //========================================================================
      // Selection of device by user
      //========================================================================


         if(idx==numUsbDevs) 
                { printf("\n\nselect the device :  ");
                  scanf("%d",&idx);
             if(idx > numUsbDevs)
                 {printf("Invalid input, Quitting.............");
                  break; }    
                
           devPtr = devList[idx-1];


           retVal = libusb_open (devPtr, &devHandle);
               if (retVal != LIBUSB_SUCCESS)
               break;


          retVal = libusb_get_device_descriptor (devPtr, &devDesc);
               if (retVal != LIBUSB_SUCCESS)
               break;
                   printf ("  \nProductid     = %d", devDesc.idProduct);
                   printf ("  \nVendorid      = %d", devDesc.idVendor);


             unsigned char data[4] ; //data to read
      
             //data[0]='a';data[1]='b';data[2]='c';data[3]='d'; //some dummy values
            
             int r; //for return values 
              r = libusb_claim_interface(devHandle, 4); //claim interface 0 (the first) of device


          if(r < 0) {


        printf("\nCannot Claim Interface");


        return 1;
                    }


        printf("\nClaimed Interface");


       int actual_length; //used to find out how many bytes were written


 
     r = libusb_bulk_transfer(devHandle,LIBUSB_ENDPOINT_IN, data, 4, &actual_length, 0);
        
          if (r == 0 && actual_length == sizeof(data)) {


                  // results of the transaction can now be found in the data buffer
                 // parse them here and report button press
              } 
          else {
                 error();
               } 




     r = libusb_release_interface(devHandle, 1); //release the claimed interface


       if(r!=0) {


                printf("\nCannot Release Interface");


                return 1;
               }


    printf("\nReleased Interface");




                idx=numUsbDevs +2;
                }


   }  // end of while loop


   if (devHandle != NULL)
   {
      //========================================================================
      // Close device if left open due to break out of loop on error.
      //========================================================================


      libusb_close (devHandle);
   }   


                   
   libusb_exit (ctx); //close the session


   printf ("\n*************************\n        Done\n*************************\n");
   return 0;
}


//==========================================
// EOF
//==========================================
