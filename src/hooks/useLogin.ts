import { useMutation } from "@tanstack/react-query";
import { signIn } from "@/api/auth";

export const useLogin = () => useMutation({ mutationFn: signIn });
