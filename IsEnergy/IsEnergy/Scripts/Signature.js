//добавим скрипт подписи

function SignDocuments(key) {

    var hashSignDocumentTemp;
    var certSubjectName;
    LoadingPanel.Show();
    $.get('/DocumentFlow/HashSignDocumentTemp', { IdDocumentTemp: key }, function (data) {
        hashSignDocumentTemp = data;
        $.post('/DocumentFlow/CertSubjectNameSignDocumentTemp', { IdDocumentTemp: key }, function (data) { 
            certSubjectName = data;
           var singdata = SignCreate(certSubjectName, hashSignDocumentTemp);
           $.post('/DocumentFlow/SaveDocumentTempInRealAndSend', { IdDocumentTemp: key, dataSing: singdata }, function (data) {
               if (data == 'True')
               {
                   $.post('/DocumentFlow/GridViewDocuments', function (data) {
                       $('#DocumentPanel').html(data);
                        LoadingPanel.Hide();
                   });
               }
           });
        });
    });
}