
$(document).ready(function () {

   if (!(_commonlib) || !(_commonlib.addListener)) {
      _commonlib = new CommonLib();
   }
   _commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_Site);

});

function ProcessAJAXCallbackResults_Site(responseData) {

   if (!(responseData) || !(responseData.parameters) || !(responseData.parameters.cmd)) {
      return;
   }

   switch ((responseData.parameters.cmd || '').toLowerCase()) {

      case 'site':

         break;
   }
}
