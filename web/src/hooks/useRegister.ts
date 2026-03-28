import { useMutation } from "@tanstack/react-query";
import { signUp } from "@/api/auth";

export const useRegister = () => useMutation({ mutationFn: signUp });
