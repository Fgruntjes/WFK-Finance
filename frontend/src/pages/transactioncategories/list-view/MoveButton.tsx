import { Button } from "antd";

export type MoveButtonProps = {
  icon: React.ReactNode;
  onClick?: () => void;
};

export function MoveButton({ icon, onClick }: MoveButtonProps) {
  return (
    <Button
      type="text"
      icon={icon}
      disabled={onClick === undefined}
      onClick={() => (onClick ? onClick() : undefined)}
    />
  );
}
