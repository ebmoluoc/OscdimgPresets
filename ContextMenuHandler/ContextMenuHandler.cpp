#include "pch.h"
#include "ContextMenuHandler.h"

///////////////////////////////////////////////////////////////////////////////////////////////////

ContextMenuHandler::ContextMenuHandler(Dll& dll) : dll_{ dll }, refCount_{ 1 }, hBitmapMenu_{ nullptr }
{
	dll_.AddRef();

	hBitmapMenu_ = hlp::BitmapFromIconResource(dll_.Handle(), IDI_ICON_MENU, 0, 0);
}

///////////////////////////////////////////////////////////////////////////////////////////////////

ContextMenuHandler::~ContextMenuHandler()
{
	if (hBitmapMenu_ != nullptr)
	{
		DeleteObject(hBitmapMenu_);
	}

	dll_.Release();
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP ContextMenuHandler::QueryInterface(REFIID riid, LPVOID* ppvObject)
{
	if (ppvObject == nullptr)
	{
		return E_POINTER;
	}
	else if (IsEqualIID(riid, IID_IContextMenu))
	{
		*ppvObject = static_cast<LPCONTEXTMENU>(this);
	}
	else if (IsEqualIID(riid, IID_IShellExtInit))
	{
		*ppvObject = static_cast<LPSHELLEXTINIT>(this);
	}
	else
	{
		*ppvObject = nullptr;
		return E_NOINTERFACE;
	}

	this->AddRef();

	return S_OK;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP_(ULONG) ContextMenuHandler::AddRef()
{
	return ++refCount_;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP_(ULONG) ContextMenuHandler::Release()
{
	auto rc{ --refCount_ };
	if (rc == 0)
	{
		delete this;
	}
	return rc;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP ContextMenuHandler::Initialize(PCIDLIST_ABSOLUTE pidlFolder, LPDATAOBJECT pdtobj, HKEY hkeyProgID)
{
	UNREFERENCED_PARAMETER(pidlFolder);
	UNREFERENCED_PARAMETER(hkeyProgID);

	if (pdtobj == nullptr)
	{
		return E_INVALIDARG;
	}

	if (pathList_.Load(pdtobj))
	{
		if (!pathList_.IsMultiItems())
		{
			return S_OK;
		}
	}

	return E_FAIL;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP ContextMenuHandler::QueryContextMenu(HMENU hmenu, UINT indexMenu, UINT idCmdFirst, UINT idCmdLast, UINT uFlags)
{
	UNREFERENCED_PARAMETER(idCmdLast);

	if ((uFlags & CMF_DEFAULTONLY) != 0)
	{
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	}

	auto hSubmenu{ CreatePopupMenu() };
	if (hSubmenu == nullptr)
	{
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	}

	GetSubmenuList();
	auto idCmd{ idCmdFirst };
	auto subMenuCount{ submenuList_.size() };

	for (UINT i = 0; i < subMenuCount; ++i)
	{
		if (!hlp::AddMenuItem(hSubmenu, i, submenuList_[i].c_str(), idCmd++, nullptr, hBitmapMenu_))
		{
			DestroyMenu(hSubmenu);
			return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
		}
	}

	if (!hlp::AddMenuItem(hmenu, indexMenu, PRODUCT_NAME_W, idCmd++, hSubmenu, hBitmapMenu_))
	{
		DestroyMenu(hSubmenu);
		return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, 0);
	}

	return MAKE_HRESULT(SEVERITY_SUCCESS, FACILITY_NULL, idCmd - idCmdFirst);
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP ContextMenuHandler::InvokeCommand(LPCMINVOKECOMMANDINFO pici)
{
	if (HIWORD(pici->lpVerb) != 0)
	{
		return E_FAIL;
	}

	auto menuIndex{ LOWORD(pici->lpVerb) };
	auto presetName{ submenuList_[menuIndex] };

	auto sourcePath{ pathList_.GetItems()[0] };
	auto exePath{ hlp::RenamePath(dll_.Path(), L"OscdimgPresets.exe") };

	auto argPreset{ hlp::EscapeArgument(presetName) };
	auto argSource{ hlp::EscapeArgument(sourcePath) };

	auto arguments{ L"-p:" + argPreset + L" -s:" + argSource + L" -c" };

	ShellExecuteW(nullptr, nullptr, exePath.c_str(), arguments.c_str(), nullptr, SW_SHOW);

	return S_OK;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

IFACEMETHODIMP ContextMenuHandler::GetCommandString(UINT_PTR idCmd, UINT uType, UINT* pReserved, LPSTR pszName, UINT cchMax)
{
	UNREFERENCED_PARAMETER(idCmd);
	UNREFERENCED_PARAMETER(uType);
	UNREFERENCED_PARAMETER(pReserved);
	UNREFERENCED_PARAMETER(pszName);
	UNREFERENCED_PARAMETER(cchMax);

	return E_NOTIMPL;
}

///////////////////////////////////////////////////////////////////////////////////////////////////

void ContextMenuHandler::GetSubmenuList()
{
	static LPCWSTR DATA_NODE{ L"PresetModel" };

	std::wstring xmlPath{ hlp::GetFilePath(FOLDERID_RoamingAppData, PRODUCT_NAME_W, L"Presets.xml") };
	if (!xmlPath.empty())
	{
		pugi::xml_document doc{};
		pugi::xml_parse_result parse{ doc.load_file(xmlPath.c_str()) };
		if (parse.status == pugi::xml_parse_status::status_ok)
		{
			pugi::xml_node arraynode{ doc.child(L"ArrayOfPresetModel") };
			if (arraynode != nullptr)
			{
				pugi::xml_node datanode{ arraynode.child(DATA_NODE) };
				while (datanode != nullptr)
				{
					std::wstring name{ datanode.child_value(L"Name") };
					if (!name.empty())
					{
						submenuList_.push_back(name);
					}
					datanode = datanode.next_sibling(DATA_NODE);
				}
			}
		}
	}

	if (submenuList_.empty())
	{
		submenuList_.push_back(L"Default");
	}
}
