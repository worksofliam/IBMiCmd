PGM          PARM(&FILE)                                                   
DCL          VAR(&FILE) TYPE(*CHAR) LEN(10)                                
DCL          VAR(&USER) TYPE(*CHAR) LEN(10)                                
             RTVJOBA    CURUSER(&USER)                                        
             DLTF       FILE(QTEMP/&FILE)                                  
             MONMSG     MSGID(CPF2105)                                     
             CRTPF      FILE(QTEMP/&FILE) RCDLEN(1730) +                   
                          FILETYPE(*SRC) CCSID(*JOB)                       
             DSPFFD     FILE(*LIBL/&FILE) OUTPUT(*OUTFILE) +               
                          OUTFILE(QTEMP/TMP)                               
             CPYF       FROMFILE(QTEMP/TMP) TOFILE(QTEMP/&FILE) +          
                          MBROPT(*REPLACE) FMTOPT(*CVTSRC)                 
             CPYTOSTMF    FROMMBR('/QSYS.LIB/QTEMP.LIB/' *CAT &FILE +      
                         *TCAT '.FILE/' *CAT &FILE *TCAT '.MBR') +         
                         TOSTMF('/HOME/' +                                 
                         *CAT &USER *TCAT '/' *CAT &FILE *TCAT '.TMP') +   
                         STMFOPT(*REPLACE)                                 
             DLTF       FILE(QTEMP/&FILE)                                  
             DLTF       FILE(QTEMP/TMP)                                                                   
ENDPGM                                                                     