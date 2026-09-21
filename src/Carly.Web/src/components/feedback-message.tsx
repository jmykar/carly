interface FeedbackMessageProps {
  error?: string;
  message?: string;
}

export function FeedbackMessage({ error, message }: FeedbackMessageProps) {
  if (error !== undefined) {
    return (
      <p className="rounded-md bg-red-50 p-3 text-sm text-red-800" role="alert">
        {error}
      </p>
    );
  }

  if (message !== undefined) {
    return (
      <p className="rounded-md bg-emerald-50 p-3 text-sm text-emerald-800" role="status">
        {message}
      </p>
    );
  }

  return null;
}
