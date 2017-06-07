**FREE
/include QRPGLESRC.NPPCTLOPT
// Part of the IBMiCmd server features
//
// Put command definition in the users IFS Folder in a file named as command.cdml
//
// Uses system API https://www.ibm.com/support/knowledgecenter/ssw_ibm_i_73/apis/qcdrcmdd.htm
//

dcl-ds myPSDS psds;
    
end-ds

dcl-proc main;
    dcl-pi *N;      
        command char(20);
    end-pi;
    
    dcl-ds RetreiveCDML_DestinationFormat qualified;
        BytesReturned  int(10);
        BytesAvailable int(10);
        CommandSource  char(30000);
    end-ds;
    dcl-ds APIError qualified;
        BytesProvided  int(10);
        BytesAvailable int(10);
        ExceptionID    char(7);
        Reserved       char(1);
        ExceptionData  char(30000);
    end-ds;
    dcl-ds PathName qualified inz;  
        CCSID                       int(10);
        CountryOrRegionID           char(2);
        LanguageID                  char(3);
        *N                          char(3);
        PathTypeIndicator           int(10);
        LengthOfPathName            int(10);
        PathNameDelimiterCharacter  char(2);
        *N                          char(10);
        Path                        char(150);
    end-ds;
    
    dcl-c FORMAT_STREAM_FILE 'DEST0200';
    dcl-c RECEIVER_FORMAT 'CMDD0100';
    
    dcl-pr RetreiveCDML extpgm('QCDRCMDD');
        QualifiedCommandName    char(20);
        OutputFile              likeds(PathName);
        DestinationFormat       char(8);
        Data                    likeds(RetreiveCDML_DestinationFormat);
        ReceiverFormat          char(8);
        Error                   likeds(APIError);
    end-pr;
    
	dcl-s delimiter char(2) inz('/'); 
    dcl-s outputFilePath char(150) inz;
    
    outputFilePath = delimiter + 'home' delimiter + %trimR(currentUser) + delimiter + trimR(command) + '.cdml';
    
	PathName.CCSID = 1208;
	PathName.CountryOrRegionID = 'GB';
	PathName.LanguageID = 'EN';
	PathName.PathTypeIndicator = TODO;
	PathName.LengthOfPathName = %len(%trimR(outputFilePath));
	PathName.PathNameDelimiterCharacter = delimiter; 
	PathName.Path = %trimR(outputFilePath) + x'00';

    callp RetreiveCDML( command : PathName : FORMAT_STREAM_FILE : Data : RECEIVER_FORMAT : APIError );
     
    return;
    
end-proc;