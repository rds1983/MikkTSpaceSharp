#pragma once

#include <memory>

#include "mikktspace.h"

using namespace System;
using namespace System::Runtime::InteropServices;

namespace PlMpegNativeWrapper {
	std::shared_ptr<plm_t> _plm;

	public ref class Native
	{
	public:
		static bool create_with_file(String^ path)
		{
			char* str2 = (char*)(void*)Marshal::StringToHGlobalAnsi(path);
			auto plm = plm_create_with_filename(str2);
			Marshal::FreeHGlobal((IntPtr)str2);

			if (!plm)
			{
				return false;
			}

			_plm = std::shared_ptr<plm_t>(plm);

			return true;
		}

		static bool create_with_memory(array<unsigned char>^ bytes)
		{
			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char* ptr = (unsigned char*)p;

			auto plm = plm_create_with_memory(ptr, bytes->Length, 0);
			if (!plm)
			{
				return false;
			}
			_plm = std::shared_ptr<plm_t>(plm);

			return true;
		}

		static void set_audio_enabled(bool enabled)
		{
			plm_set_audio_enabled(_plm.get(), enabled ? 1 : 0);
		}

		static bool get_next_frame_rgb(array<unsigned char>^ bytes)
		{
			plm_frame_t* frame = plm_decode_video(_plm.get());
			if (!frame)
			{
				return false;
			}

			auto width = plm_get_width(_plm.get());

			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char* ptr = (unsigned char*)p;
			plm_frame_to_rgb(frame, ptr, width * 3);

			return true;
		}

		static bool get_next_frame_audio(array<float>^ data)
		{
			plm_samples_t* samples = plm_decode_audio(_plm.get());
			if (!samples)
			{
				return false;
			}

			Marshal::Copy(IntPtr((void*)samples->interleaved), data, 0, samples->count * 2);

			return true;
		}
	};
}
