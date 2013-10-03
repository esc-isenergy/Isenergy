var CADESCOM_CADES_X_LONG_TYPE_1 = 0x5d;
var CAPICOM_CURRENT_USER_STORE = 2;
var CAPICOM_MY_STORE = "My";
var CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED = 2;
var CAPICOM_CERTIFICATE_FIND_SUBJECT_NAME = 1;

function GetErrorMessage(e) {
    var err = e.message;
    if (!err) {
        err = e;
    } else if (e.number) {
        err += " (" + e.number + ")";
    }
    return err;
}

// создание подписи
function SignCreate(certSubjectName, dataToSign) {
    if (isPluginInstalled() == false) {
        LoadingPanel.Hide();
        return;
    }
    var oStore = CreateObject("CAPICOM.Store");
    oStore.Open(CAPICOM_CURRENT_USER_STORE, CAPICOM_MY_STORE,
    CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

    var oCertificate = GetCertificateBySubjectName(certSubjectName);

    var oSigner = CreateObject("CAdESCOM.CPSigner");
    oSigner.Certificate = oCertificate;
    oSigner.TSAAddress = "http://cryptopro.ru/tsp/";

    var oSignedData = CreateObject("CAdESCOM.CadesSignedData");
    oSignedData.Content = dataToSign;
   
    try {
        var sSignedMessage = oSignedData.SignCades(oSigner, CADESCOM_CADES_X_LONG_TYPE_1);
    } catch (err) {
        LoadingPanel.Hide();
        alert("Не удалось подписать документ: " + GetErrorMessage(err));
        return;
    }
    oStore.Close();

    if (Verify(sSignedMessage)) {
        return sSignedMessage;
    }
    else {
        return;
    }
}

// проверка
function Verify(sSignedMessage) {
    if (isPluginInstalled() == false) {
        return;
    }
    var oSignedData = CreateObject("CAdESCOM.CadesSignedData");
    try {
        oSignedData.VerifyCades(sSignedMessage, CADESCOM_CADES_X_LONG_TYPE_1);
    } catch (err) {
        LoadingPanel.Hide();
        alert("Не удалось выполнить проверку подписи документа: " + GetErrorMessage(err));
        return false;
    }
    return true;
}

// Функция активации объектов КриптоПро ЭЦП Browser plug-in
function CreateObject(name) {
   
    switch (navigator.appName) {
        case "Microsoft Internet Explorer":
            return new ActiveXObject(name);
        default:
            if (isMobile.iOS()) {
                return call_ru_cryptopro_npcades_10_native_bridge("CreateObject", [name]);
            }
            var cadesobject = document.getElementById("cadesplugin");
            return cadesobject.CreateObject(name);
    }
}

// поиск серта
function GetCertificateBySubjectName(certSubjectName) {
    var oStore = CreateObject("CAPICOM.Store");
    oStore.Open(CAPICOM_CURRENT_USER_STORE, CAPICOM_MY_STORE,
        CAPICOM_STORE_OPEN_MAXIMUM_ALLOWED);

    var oCertificates = oStore.Certificates.Find(
        CAPICOM_CERTIFICATE_FIND_SUBJECT_NAME, certSubjectName);
    if (oCertificates.Count == 0) {
        LoadingPanel.Hide();
        alert("Сертификат не найден: " + certSubjectName);
        return;
    }
    var oCertificate = oCertificates.Item(1);

    return oCertificate;
}
//проверка наличия плагина
function isPluginInstalled() {
    switch (navigator.appName) {
        case 'Microsoft Internet Explorer':
            try {
                var obj = new ActiveXObject("CAdESCOM.CPSigner");
                return true;
            }
            catch (err) { }
            break;
        default:
            var mimetype = navigator.mimeTypes["application/x-cades"];
            if (mimetype) {
                var plugin = mimetype.enabledPlugin;
                if (plugin) {
                    return true;
                }
            }
    }
    LoadingPanel.Hide();
    confirm('На вашем компьютере не установлен плагин для работу с ЭЦП. Установить плагин?')
    {
        document.location.href = "http://www.cryptopro.ru/products/cades/plugin/get";
    }
   
    return false;
}

var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
    }
};