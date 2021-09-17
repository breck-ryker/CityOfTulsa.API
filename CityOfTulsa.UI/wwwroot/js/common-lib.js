
var _commonlib = new CommonLib();
var _ksaLastRunTimeText = null;
var _ksaLastRunTime = (new Date()).getTime();
var _ksaIntervalMinutes = 4;
var _timerKSA = setInterval('_KeepSessionAlive();', (1000 * 60 * _ksaIntervalMinutes));
var _protectSpinnerFromHiding = false;
var _timerDelayCallAJAX = 0;
var _timerSpinner = 0;
var _spinnerHideDelay = 0;
var _currentCommandText = null;
var _suppressSpinnerOnce = false;
var _pageLoadTime = new Date();
var _sessionID = null;

function CommonLib() {
   this._listeners = {};
}

$(document).ready(function () {

   setTimeout(function (e) { CallAJAX('common-lib.page_loaded', null, GetPageName(), null, null, true, null); }, 1000);

});

CommonLib.prototype = {

   constructor: CommonLib,

   addListener: function (type, listener) {

      if (typeof this._listeners[type] == 'undefined') {
         this._listeners[type] = [];
      }

      this._listeners[type].push(listener);
   },

   fire: function (e) {

      if (typeof (e) == 'string') {
         e = { type: e };
      }

      if (!(e.target)) {
         e.target = this;
      }

      if (!(e.type)) {
         throw new Error('Event object missing \'type\' property.');
      }

      var listeners = null;

      if (this._listeners[e.type] instanceof Array) {
         listeners = this._listeners[e.type];
         for (var i = 0, len = listeners.length; i < len; i++) {
            listeners[i].call(this, e);
         }
      }
      else if (e.target._listeners) {
         listeners = e.target._listeners;
         for (var i = 0, len = listeners.length; i < len; i++) {
            listeners[i].call(this, e);
         }
      }
   },

   removeListener: function (type, listener) {
      if (this._listeners[type] instanceof Array) {
         var listeners = this._listeners[type];
         for (var i = 0, len = listeners.length; i < len; i++) {
            if (listeners[i] === listener) {
               listeners.splice(i, 1);
               break;
            }
         }
      }
   },

   replaceAll: function (targetString, findString, replaceString) {

      return ReplaceAll(targetString, findString, replaceString);
   }
};

function DelayCallAJAX(
   nDelayMS,
   cmd,
   data,
   context,
   ids,
   values,
   suppressSpinnerOnce,
   dict
) {

   if (_timerDelayCallAJAX > 0) {
      clearTimeout(_timerDelayCallAJAX);
   }

   _timerDelayCallAJAX = setTimeout(
      function () {
         CallAJAX(cmd, data, context, ids, values, suppressSpinnerOnce, dict);
      },
      nDelayMS
   );
}

function CallAJAX(
   cmd,
   data,
   context,
   ids,
   values,
   suppressSpinnerOnce,
   dict
) {

   _suppressSpinnerOnce = suppressSpinnerOnce;
   _currentCommandText = cmd;

   var jsonData = {
      cmd: (cmd || ''),
      data: (data || ''),
      context: (context || ''),
      ids: (ids || []),
      values: (values || []),
      dict: (dict || {})
   }

   $(document).ready(function () {
      $.ajax({
         type: 'POST',
         url: '/AJAX/ProcessMessage',
         cache: false,
         data: JSON.stringify(jsonData),
         contentType: 'application/json',
         dataType: 'json',
         success: function (data) {
            data = JSON.parse(data);
            _ProcessAJAXCallbackResults(data);
         },
         error: function (e) {
            var dtNow = new Date();
            var nTimeDiff = (dtNow - _pageLoadTime);
            var nTimeDiffSecs = (nTimeDiff / 1000);
            if (!(_pageLoadTime) || nTimeDiffSecs >= 30) {
               ReloadPage();
            }
            else {
               alert('AJAX/ProcessMessage Error on [' + (cmd || '') + ']: ' + e.responseText);
            }
         }
      });
   });
}

function _ProcessAJAXCallbackResults(response) {

   if (response) {

      if (_commonlib && _commonlib.fire && typeof (_commonlib.fire) != 'undefined') {

         if (response.dict) {
            for (var key in response.dict) {
               if (typeof (response[key]) == 'undefined') {
                  response[key] = response.dict[key];
               }
            }
         }

         _commonlib.fire({ type: '_ProcessAJAXCallbackResults', parameters: response });    //can also do target.fire('foo');
      }
   }

   _currentCommandText = null;
}

function ReloadPage(hash) {
   if (hash) {
      window.location.hash = hash;
   }
   window.location.reload(true);
}

function GetQueryStringParms() {
   var vars = [], hash;
   var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
   for (var i = 0; i < hashes.length; i++) {
      hash = hashes[i].split('=');
      vars.push(hash[0]);
      vars[hash[0]] = hash[1];
   }
   return vars;
}

function GetQueryStringParm(parmName) {

   parmName = parmName.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');

   var regex = new RegExp('[\\?&]' + parmName + '=([^&#]*)'), results = regex.exec(window.location.search);

   return results == null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
}

function GetURLParts(bIncludeQueryString) {

   var url = window.location.href;

   url = ReplaceAll(url, '://', '/');
   url = ReplaceAll(url, ' ', '');
   url = ReplaceAll(url, ':', '_');

   var sNewURL = '';
   var sQueryString = '';
   var sURLParts = url.split('/');
   var sSubParts;

   for (var i = 0; i < sURLParts.length; i++) {

      if (sURLParts[i].length > 0) {

         if (sURLParts[i].indexOf('?') >= 0) {

            sSubParts = sURLParts[i].split('?');

            if (sSubParts.length > 1) {
               sURLParts[i] = sSubParts[0];
               sQueryString = sSubParts[1];
            }
         }

         if (sURLParts[i].indexOf('&') >= 0) {

            sSubParts = sURLParts[i].split('&');

            if (sSubParts.length > 1) {
               sURLParts[i] = sSubParts[0];
            }
         }

         if (sURLParts[i].length > 0) {

            sNewURL = sNewURL + (sNewURL.length > 0 ? ',' : '') + sURLParts[i];
         }
      }
   }

   sNewURL = sNewURL + (bIncludeQueryString && sQueryString ? ',' + sQueryString : '');
   sNewURL = encodeURI(sNewURL);
   return sNewURL.split(',');
}

function RefreshImage(img) {
   if (typeof (img) == 'undefined' || img == null) {
      return;
   }
   var sImagePath = img.src;
   var sParts = sImagePath.split('?');
   sImagePath = sParts[0];
   if (sImagePath.indexOf('?') < 0) {
      img.src = (sImagePath + '?v=' + Date());
   }
}

function ReplaceAll(targetString, findString, replaceString) {

   var nSearchIndex = 0;
   var sNewString = '';
   targetString = String(targetString);

   while (targetString.indexOf(findString, nSearchIndex) != -1) {
      //while ($.inArray(findString, ))
      sNewString += targetString.substring(nSearchIndex, targetString.indexOf(findString, nSearchIndex));
      sNewString += replaceString;
      nSearchIndex = (targetString.indexOf(findString, nSearchIndex) + findString.length);
   }

   sNewString += targetString.substring(nSearchIndex, targetString.length);
   return sNewString;
}

function Left(s, n) {
   if (n <= 0) {
      return '';
   }
   else if (n > String(s).length) {
      return s;
   }
   else {
      return String(s).substring(0, n);
   }
}

function Right(s, n) {
   if (n <= 0) {
      return '';
   }
   else if (n > String(s).length) {
      return s;
   }
   else {
      var iLen = String(s).length;
      return String(s).substring(iLen, iLen - n);
   }
}

function IsNumeric(value) {
   if (value == null || typeof (value) == 'undefined' || value == '') {
      return false;
   }
   return !(isNaN(value));
}

function RoundUpToNearest(nTarget, nToNearest) {
   return Math.ceil(nTarget / nToNearest) * nToNearest;
}

$.ajaxSetup({
   // Disable caching of AJAX responses
   cache: false
});

$(document).ajaxStart(function () {
   if (_suppressSpinnerOnce) {
      _suppressSpinnerOnce = false;
      return;
   }
   if (_timerSpinner > 0) {
      clearTimeout(_timerSpinner);
      _timerSpinner = 0;
   }
   ShowSpinner();
});

$(document).ajaxStop(function () {
   if (_spinnerHideDelay > 0) {
      setTimeout('HideSpinner();', _spinnerHideDelay);
      _spinnerHideDelay = 0;
   }
   else {
      HideSpinner();
   }
});

function ShowSpinner() {
   $('#overlay').data('lastcmd', (_currentCommandText || 'empty'));
   $('#overlay').show();
}

function HideSpinner() {
   if (_protectSpinnerFromHiding) {
      return;
   }
   $('#overlay').fadeOut(200);
}

function ClearKSATimer() {
   if (typeof (_timerKSA) != 'undefined' && _timerKSA != null && _timerKSA != 0) {
      clearInterval(_timerKSA);
   }
}

function ResetKeepAliveTimer() {

   ClearKSATimer();

   _timerKSA = setInterval('_KeepSessionAlive();', (1000 * 60 * _ksaIntervalMinutes));
}

function _KeepSessionAlive() {

   var dtCurrentTime = (new Date()).getTime();

   if (dtCurrentTime > (_ksaLastRunTime + (1000 * 60 * (_ksaIntervalMinutes + 1)))) {
      ReloadPage();  // probably just woke up
   }

   _ksaLastRunTime = dtCurrentTime;

   _suppressSpinnerOnce = true;

   $.ajax({
      type: 'POST',
      url: '/Home/KeepSessionAlive',
      cache: false,
      success: function (d) {

         _ksaLastRunTimeText = d;

         var $divKSA = $('#divKeepSessionAlive');

         if (!($divKSA) || $divKSA.length == 0) {

            $('body').append('<div id="divKeepSessionAlive" style="display:none;"></div>');

            var $divKSA = $('#divKeepSessionAlive');
         }

         if ($divKSA && $divKSA.length == 1) {
            $divKSA.text(_ksaLastRunTimeText);
         }
      }
   });
}

function ContainsKey(obj, key) {
   return (typeof obj[key] !== 'undefined');
}

function DecodeText(encodedText) {
   // Because a textarea allows no HTML markup inside it
   var textArea = document.createElement('textarea');
   textArea.innerHTML = encodedText;
   return textArea.value;
}

function GetPageName() {
   var sPath = window.location.pathname;
   var sPageName = sPath.split('/').pop();
   return sPageName;
}
