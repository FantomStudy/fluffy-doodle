"use client";

import { Dialog as BaseDialog } from "@base-ui/react/dialog";
import clsx from "clsx";
import { XIcon } from "lucide-react";
import { Button } from "../Button";
import styles from "./Dialog.module.css";

export const Dialog = ({ ...props }: BaseDialog.Root.Props) => {
  return <BaseDialog.Root {...props} />;
};

export const DialogTrigger = ({ ...props }: BaseDialog.Trigger.Props) => {
  return <BaseDialog.Trigger {...props} />;
};

export const DialogClose = ({ ...props }: BaseDialog.Close.Props) => {
  return <BaseDialog.Close {...props} />;
};

export const DialogContent = ({
  className,
  children,
  showCloseButton = true,
  ...props
}: BaseDialog.Popup.Props & {
  showCloseButton?: boolean;
}) => {
  return (
    <BaseDialog.Portal>
      <BaseDialog.Backdrop className={styles.backdrop} />
      <BaseDialog.Popup className={clsx(styles.content, className)} {...props}>
        {children}
        {showCloseButton && (
          <BaseDialog.Close
            render={
              <Button variant="ghost" size="icon" className={styles.close}>
                <XIcon />
                <span className="sr-only">Close</span>
              </Button>
            }
          />
        )}
      </BaseDialog.Popup>
    </BaseDialog.Portal>
  );
};

export const DialogHeader = ({ className, ...props }: React.ComponentProps<"div">) => {
  return <div className={clsx(styles.header, className)} {...props} />;
};

export const DialogFooter = ({ className, ...props }: React.ComponentProps<"div">) => {
  return <div className={clsx(styles.footer, className)} {...props} />;
};

export const DialogTitle = ({ className, ...props }: BaseDialog.Title.Props) => {
  return <BaseDialog.Title className={clsx(styles.title, className)} {...props} />;
};

export const DialogDescription = ({ className, ...props }: BaseDialog.Description.Props) => {
  return <BaseDialog.Description className={clsx(styles.description, className)} {...props} />;
};
