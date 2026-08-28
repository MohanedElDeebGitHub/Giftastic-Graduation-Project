package com.giftastic.giftastic.modules.identity.service;

import com.giftastic.giftastic.common.security.JwtUtils;
import com.giftastic.giftastic.common.security.UserPrincipal;
import com.giftastic.giftastic.modules.identity.dto.AuthResponse;
import lombok.RequiredArgsConstructor;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.stereotype.Service;

@Service
@RequiredArgsConstructor
public class AuthService {

    private final AuthenticationManager authManager;
    private final JwtUtils jwtUtils;

    public AuthResponse login(String email, String password) {
        Authentication auth = authManager.authenticate(
                new UsernamePasswordAuthenticationToken(email, password)
        );

        UserPrincipal principal = (UserPrincipal) auth.getPrincipal();
        String token = jwtUtils.generateToken(principal);

        // Extract roles as strings
        java.util.List<String> roles = principal.getAuthorities().stream()
                .map(Object::toString)
                .toList();

        // Determine primary role
        String primaryRole = roles.stream()
                .filter(r -> r.startsWith("ROLE_"))
                .findFirst()
                .map(r -> r.replace("ROLE_", ""))
                .orElse("CUSTOMER");

        // Build user info
        AuthResponse.UserInfo userInfo = new AuthResponse.UserInfo(
                principal.getUserId(),
                principal.getEmail(),
                principal.getSupplierId(),
                roles,
                primaryRole
        );

        return new AuthResponse(token, principal.getEmail(), principal.getUserId(), userInfo);
    }
}