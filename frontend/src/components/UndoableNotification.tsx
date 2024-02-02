type UndoableNotificationProps = {
  message: string;
  cancelMutation?: () => void;
  closeToast?: () => void;
};

function UndoableNotification({
  message,
  cancelMutation,
  closeToast,
}: UndoableNotificationProps) {
  return (
    <div>
      <p>{message}</p>
      <button
        onClick={() => {
          cancelMutation?.();
          closeToast?.();
        }}
      >
        Undo
      </button>
    </div>
  );
}

export default UndoableNotification;
