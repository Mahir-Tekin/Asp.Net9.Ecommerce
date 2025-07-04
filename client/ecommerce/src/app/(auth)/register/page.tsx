'use client';

import { useState, Suspense } from 'react';
import { useRouter, useSearchParams } from 'next/navigation';
import Link from 'next/link';
import { useAuth } from '@/context/AuthContext';
import { ValidationError } from '@/types/auth';
import { useFormStatus } from 'react-dom';

// Submit Button Component with automatic loading state
function SubmitButton() {
    const { pending } = useFormStatus();
    
    return (
        <button
            type="submit"
            disabled={pending}
            className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
        >
            {pending ? 'Creating account...' : 'Sign up'}
        </button>
    );
}

function RegisterPageContent() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const { register } = useAuth();
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<ValidationError[]>([]);
    
    const redirectTo = searchParams.get('redirect');

    const handleSubmit = async (formData: FormData) => {
        const email = formData.get('email') as string;
        const password = formData.get('password') as string;
        const confirmPassword = formData.get('confirmPassword') as string;

        // Basic password match validation
        if (password !== confirmPassword) {
            setError(null);
            setValidationErrors([{
                propertyName: 'confirmPassword',
                errorMessage: 'Passwords do not match'
            }]);
            return;
        }

        try {
            const result = await register({
                email,
                password,
                confirmPassword
            });
            
            if (result.success) {
                // If there's a redirect parameter and it's not an admin user,
                // we can redirect manually to the desired page
                if (redirectTo && redirectTo !== '/admin/dashboard') {
                    router.push(redirectTo);
                }
                // Otherwise, the AuthContext register function handles the redirect automatically
                return;
            }

            setError(result.error || null);
            setValidationErrors(result.validationErrors || []);
        } catch {
            setError('An unexpected error occurred during registration');
            setValidationErrors([]);
        }
    };

    // Helper function to get validation error for a field
    const getFieldError = (fieldName: string) => {
        return validationErrors?.find(
            error => error.propertyName.toLowerCase() === fieldName.toLowerCase()
        )?.errorMessage;
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8">
                <div>
                    <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
                        Create your account
                    </h2>
                </div>
                <form action={handleSubmit} className="mt-8 space-y-6">
                    {/* Show general error */}
                    {error && (
                        <div className="rounded-md bg-red-50 p-4">
                            <div className="flex">
                                <div className="flex-shrink-0">
                                    <svg className="h-5 w-5 text-red-400" viewBox="0 0 20 20" fill="currentColor">
                                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                                    </svg>
                                </div>
                                <div className="ml-3">
                                    <h3 className="text-sm font-medium text-red-800">
                                        Registration Error
                                    </h3>
                                    <div className="mt-2 text-sm text-red-700">
                                        {error}
                                    </div>
                                </div>
                            </div>
                        </div>
                    )}
                    <div className="rounded-md shadow-sm -space-y-px">
                        <div>
                            <label htmlFor="email" className="sr-only">Email address</label>
                            <input
                                id="email"
                                name="email"
                                type="email"
                                required
                                className={`appearance-none rounded-none relative block w-full px-3 py-2 border ${
                                    getFieldError('email') ? 'border-red-300' : 'border-gray-300'
                                } placeholder-gray-500 text-gray-900 rounded-t-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm`}
                                placeholder="Email address"
                            />
                            {getFieldError('email') && (
                                <p className="mt-1 text-sm text-red-600">
                                    {getFieldError('email')}
                                </p>
                            )}
                        </div>
                        <div>
                            <label htmlFor="password" className="sr-only">Password</label>
                            <input
                                id="password"
                                name="password"
                                type="password"
                                required
                                className={`appearance-none rounded-none relative block w-full px-3 py-2 border ${
                                    getFieldError('password') ? 'border-red-300' : 'border-gray-300'
                                } placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm`}
                                placeholder="Password"
                            />
                            {getFieldError('password') && (
                                <p className="mt-1 text-sm text-red-600">
                                    {getFieldError('password')}
                                </p>
                            )}
                        </div>
                        <div>
                            <label htmlFor="confirmPassword" className="sr-only">Confirm Password</label>
                            <input
                                id="confirmPassword"
                                name="confirmPassword"
                                type="password"
                                required
                                className={`appearance-none rounded-none relative block w-full px-3 py-2 border ${
                                    getFieldError('confirmPassword') ? 'border-red-300' : 'border-gray-300'
                                } placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 focus:z-10 sm:text-sm`}
                                placeholder="Confirm Password"
                            />
                            {getFieldError('confirmPassword') && (
                                <p className="mt-1 text-sm text-red-600">
                                    {getFieldError('confirmPassword')}
                                </p>
                            )}
                        </div>
                    </div>

                    <SubmitButton />
                </form>

                <div className="text-center">
                    <Link href="/login" className="text-sm text-indigo-600 hover:text-indigo-500">
                        Already have an account? Sign in
                    </Link>
                </div>
            </div>
        </div>
    );
}

export default function RegisterPage() {
    return (
        <Suspense fallback={<div className="flex items-center justify-center min-h-screen">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-indigo-600"></div>
        </div>}>
            <RegisterPageContent />
        </Suspense>
    );
} 