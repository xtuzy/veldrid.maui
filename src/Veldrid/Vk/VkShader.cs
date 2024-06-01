﻿//using Vulkan;
//using static Vulkan.VulkanNative;
using static Veldrid.Vk.VulkanUtil;
using System;

namespace Veldrid.Vk
{
    internal unsafe class VkShader : Shader
    {
        private readonly VkGraphicsDevice _gd;
        private readonly VkShaderModule _shaderModule;
        private bool _disposed;
        private string _name;

        public VkShaderModule ShaderModule => _shaderModule;

        public override bool IsDisposed => _disposed;

        public VkShader(VkGraphicsDevice gd, ref ShaderDescription description)
            : base(description.Stage, description.EntryPoint)
        {
            _gd = gd;

            VkShaderModuleCreateInfo shaderModuleCI = new VkShaderModuleCreateInfo();
            fixed (byte* codePtr = description.ShaderBytes)
            {
                shaderModuleCI.CodeSize = (UIntPtr)description.ShaderBytes.Length;
                shaderModuleCI.PCode = (uint*)codePtr;
                VkResult result = vk.GetApi().CreateShaderModule(gd.Device, ref shaderModuleCI, null, out _shaderModule);
                CheckResult(result);
            }
        }

        public override string Name
        {
            get => _name;
            set
            {
                _name = value;
                _gd.SetResourceName(this, value);
            }
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                vk.GetApi().DestroyShaderModule(_gd.Device, ShaderModule, null);
            }
        }
    }
}
