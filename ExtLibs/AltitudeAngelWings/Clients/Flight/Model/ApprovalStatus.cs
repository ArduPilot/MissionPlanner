using System.Collections.Generic;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class ApprovalStatus
    {
        public ApprovalStatus(ApprovalState state, string approverState = null
        )
        {
            State = state;
            ApproverState = approverState;
        }

        /// <summary>
        /// The canonical state of the approval
        /// </summary>
        public ApprovalState State { get; }

        /// <summary>
        /// The local approver state.
        /// </summary>
        public string ApproverState { get; }

        /// <summary>
        /// A human friendly message explaining the state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Indicates that the operator must perform an action to continue with this request.
        /// </summary>
        public bool ActionRequired { get; set; } = false;

        /// <summary>
        /// Whether this approval status should be keep workflow decisions after an edit has taken place.
        /// If false, a new workflow will be generated after a edit and existing workflow decisions will be cleared.
        /// If true, the workflow can continue with existing decisions after an edit.
        /// </summary>
        public bool ShouldKeepWorkflowDecisionsAfterEdit { get; set; } = false;

        /// <summary>
        /// The workflow instance ID this approval status is related to.
        /// </summary>
        public string WorkflowInstanceId { get; set; } = null;


        /// <summary>
        /// I interaction is required to continue, this will contain details of actions available to the user.
        /// </summary>
        public List<ApprovalAction> Actions { get; set; } = new List<ApprovalAction>();
    }
}