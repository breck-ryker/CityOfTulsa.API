
$(document).ready(function () {

   if (!(_commonlib) || !(_commonlib.addListener)) {
      _commonlib = new CommonLib();
   }
   _commonlib.addListener('_ProcessAJAXCallbackResults', ProcessAJAXCallbackResults_Site);

   $('.tooltipster').tooltipster({
      animation: 'fade',
      delay: 100,
      theme: 'tooltipster-default',
      touchDevices: false,
      trigger: 'hover'
   });

   $('.tooltipsterLeft').tooltipster({
      animation: 'fade',
      delay: 100,
      theme: 'tooltipster-default',
      touchDevices: false,
      trigger: 'hover',
      position: 'left'
   });

   $('a.nav-link,.blockui').off('click').on('click', function (e) {
      ShowSpinner();
   });

   //$('input[type = submit]').click(function () {
   //   $.blockUI({
   //      css: {
   //         border: 'none',
   //         padding: '15px',
   //         backgroundColor: '#000',
   //         opacity: .6
   //      }
   //   });
   //});

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
