window.gpt4Interop = {
    executeJavaScript: function (jsCode) {
        eval(jsCode);
    },
    startSpeechRecognition: async function (dotNetRef) {
        if (!('webkitSpeechRecognition' in window)) {
            return false;
        }

        const recognition = new webkitSpeechRecognition();
        recognition.lang = 'pt-BR';
        recognition.interimResults = false;
        recognition.maxAlternatives = 1;
        recognition.continuous = false;

        recognition.onresult = function (event) {
            const transcript = event.results[0][0].transcript;
            dotNetRef.invokeMethodAsync('SetVoiceInput', transcript);
        };

        recognition.onspeechend = function () {
            recognition.stop();
        };

        recognition.start();
        return true;
    }
};