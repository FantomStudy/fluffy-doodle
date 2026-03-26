import { Button as BaseButton } from "@base-ui/react/button";
import clsx from "clsx";
import styles from "./Button.module.css";

interface ButtonProps extends BaseButton.Props {
  variant?: "primary" | "ghost" | "danger" | "link";
  size?: "md" | "icon" | "icon-sm";
}

export const Button = ({ variant = "primary", size = "md", className, ...props }: ButtonProps) => {
  return (
    <BaseButton
      className={clsx(styles.button, styles[variant], styles[size], className)}
      {...props}
    />
  );
};
