import 'package:flutter_bloc/flutter_bloc.dart';
import '../../domain/repositories/auth_repository.dart';
import 'auth_event.dart';
import 'auth_state.dart';

/// BLoC for handling authentication state
class AuthBloc extends Bloc<AuthEvent, AuthState> {
  final AuthRepository _authRepository;

  AuthBloc(this._authRepository) : super(AuthInitial()) {
    on<CheckAuthStatusEvent>(_onCheckAuthStatus);
    on<RegisterEvent>(_onRegister);
    on<LoginEvent>(_onLogin);
    on<LogoutEvent>(_onLogout);
  }

  /// Handle checking authentication status
  Future<void> _onCheckAuthStatus(
    CheckAuthStatusEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());
    
    final isAuthenticated = await _authRepository.isAuthenticated();
    
    if (isAuthenticated) {
      final userResult = await _authRepository.getCurrentUser();
      
      userResult.fold(
        (failure) => emit(AuthError(message: failure.message, data: failure.data)),
        (user) {
          if (user != null) {
            emit(Authenticated(user));
          } else {
            emit(Unauthenticated());
          }
        },
      );
    } else {
      emit(Unauthenticated());
    }
  }

  /// Handle user registration
  Future<void> _onRegister(
    RegisterEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());
    
    final result = await _authRepository.register(
      email: event.email,
      password: event.password,
      firstName: event.firstName,
      lastName: event.lastName,
    );
    
    result.fold(
      (failure) => emit(AuthError(message: failure.message, data: failure.data)),
      (user) {
        emit(RegisterSuccess(user));
        emit(Authenticated(user));
      },
    );
  }

  /// Handle user login
  Future<void> _onLogin(
    LoginEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());
    
    final result = await _authRepository.login(
      email: event.email,
      password: event.password,
    );
    
    result.fold(
      (failure) => emit(AuthError(message: failure.message, data: failure.data)),
      (user) {
        emit(LoginSuccess(user));
        emit(Authenticated(user));
      },
    );
  }

  /// Handle user logout
  Future<void> _onLogout(
    LogoutEvent event,
    Emitter<AuthState> emit,
  ) async {
    emit(AuthLoading());
    
    final result = await _authRepository.logout();
    
    result.fold(
      (failure) => emit(AuthError(message: failure.message, data: failure.data)),
      (success) {
        emit(LogoutSuccess());
        emit(Unauthenticated());
      },
    );
  }
}
