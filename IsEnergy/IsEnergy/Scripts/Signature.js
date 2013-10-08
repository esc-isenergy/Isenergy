//добавим скрипт подписи

function SignPlagin(data) {
    var singdata = SignCreate(data.CertSubjectName, data.HashDocument);
    $.post('/DocumentFlow/SaveDocumentTempInRealAndSend', { IdDocumentTemp: data.iddocumenttemp, dataSing: singdata }, function (data) {
               if (data == 'True')
               {
                   $.post('/DocumentFlow/GridViewDocuments', function (data) {
                       $('#DocumentPanel').html(data);
                        LoadingPanel.Hide();
                   });
               }
           });


}