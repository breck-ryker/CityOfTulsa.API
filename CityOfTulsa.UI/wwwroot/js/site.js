
$(document).ready(function () {

   if (!(_commonlib) || !(_commonlib.addListener)) {
      _commonlib = new CommonLib();
   }
   _commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_Site);

   $('a.nav-link').off('click').on('click', function (e) {
      ShowSpinner();
   });

   $('.cot-cmd-btn').off('click').on('click', function (e) {
      var $this = $(this);
      var cmd = $this.data('cotcmd');
      if (cmd) {
         CallAJAX(cmd, null, null, null, null, false);
      }
   });
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
