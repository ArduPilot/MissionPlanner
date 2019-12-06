@[if msg.kind == msg.KIND_SERVICE]@
@{from canard_dsdlc_helpers import *}@
namespace UAVCAN
{
	public partial class uavcan {

		const double @(underscored_name(msg).upper())_DT_ID = @(msg.default_dtid);
		const double @(underscored_name(msg).upper())_DT_SIG = @('0x%08X' % (msg.get_data_type_signature(), ));
	}
}
@[end if]@
