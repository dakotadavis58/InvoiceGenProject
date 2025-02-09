"use client";

import Link from "next/link";
import { useFormStatus } from "react-dom";
import { useActionState } from "react";

interface Field {
  id: string;
  label: string;
  type: string;
  placeholder: string;
}

interface FormState {
  error?: string;
}

// The action type should match our server actions
type AuthAction = (state: FormState, formData: FormData) => Promise<FormState>;

interface AuthFormProps {
  title: string;
  fields: Field[];
  submitLabel: string;
  alternateLink: {
    text: string;
    href: string;
    label: string;
  };
  action: AuthAction;
}

function SubmitButton({ label }: { label: string }) {
  const { pending } = useFormStatus();

  return (
    <button
      type="submit"
      disabled={pending}
      className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
    >
      {pending ? "Please wait..." : label}
    </button>
  );
}

export function AuthForm({
  title,
  fields,
  submitLabel,
  alternateLink,
  action,
}: AuthFormProps) {
  const [state, formAction] = useActionState<FormState, FormData>(action, {
    error: "",
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="max-w-md w-full space-y-8 p-8 bg-white rounded-lg shadow">
        <div>
          <h2 className="text-center text-3xl font-extrabold text-gray-900">
            {title}
          </h2>
        </div>
        <form className="mt-8 space-y-6" action={formAction}>
          {state.error && (
            <div className="bg-red-50 text-red-500 p-3 rounded">
              {state.error}
            </div>
          )}
          <div className="rounded-md shadow-sm space-y-4">
            {fields.map((field) => (
              <div key={field.id}>
                <label htmlFor={field.id} className="sr-only">
                  {field.label}
                </label>
                <input
                  id={field.id}
                  name={field.id}
                  type={field.type}
                  required
                  className="appearance-none rounded relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm"
                  placeholder={field.placeholder}
                />
              </div>
            ))}
          </div>

          <div>
            <SubmitButton label={submitLabel} />
          </div>
        </form>
        <div className="text-center">
          <Link
            href={alternateLink.href}
            className="font-medium text-indigo-600 hover:text-indigo-500"
          >
            {alternateLink.text} {alternateLink.label}
          </Link>
        </div>
      </div>
    </div>
  );
}
