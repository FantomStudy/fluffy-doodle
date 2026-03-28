import clsx from "clsx";
import styles from "./Badge.module.css";

interface BadgeProps extends React.ComponentProps<"span"> {
  variant?: "primary" | "success" | "lock";
}

export const Badge = ({ variant = "primary", className, ...props }: BadgeProps) => {
  return <span className={clsx(styles.badge, styles[variant], className)} {...props} />;
};
